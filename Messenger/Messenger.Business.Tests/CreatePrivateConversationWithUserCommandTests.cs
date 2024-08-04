using AutoMapper;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Profiles;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Moq;
using System.Net;

namespace Messenger.Business.Tests;

[TestFixture]

public class CreatePrivateConversationWithUserCommandTests
{
    private IMapper _mapper;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IHubService> _hubServiceMock;
    private CreatePrivateConversationWithUserCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _hubServiceMock = new Mock<IHubService>();
        _handler = new CreatePrivateConversationWithUserCommandHandler(
            _unitOfWorkMock.Object,
            _mapper
        );
    }

    [Test]
    public async Task Handle_ShouldReturnConflict_WhenConversationAlreadyExists()
    {
        // Arrange
        var command = new CreatePrivateConversationWithUserCommand
        {
            CreatorUserId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var existingConversation = new Conversation();

        _unitOfWorkMock.Setup(u => u.Conversations.GetPrivateConversationWithUserAsync(command.CreatorUserId, command.UserId))
            .ReturnsAsync(existingConversation);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.Conflict, result.HttpStatusCode);
        Assert.AreEqual("Conversation with this user already exists.", result.ErrorMessage);
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new CreatePrivateConversationWithUserCommand
        {
            CreatorUserId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetPrivateConversationWithUserAsync(command.CreatorUserId, command.UserId))
            .ReturnsAsync((Conversation)null);

        _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(command.UserId))
            .ReturnsAsync((User)null); 

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.AreEqual("User was not found.", result.ErrorMessage);
    }

    [Test]
    public async Task Handle_ShouldCreateConversation_WhenConversationDoesNotExist()
    {
        // Arrange
        var command = new CreatePrivateConversationWithUserCommand
        {
            CreatorUserId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var creatorUser = new User { Id = command.CreatorUserId };
        var targetUser = new User { Id = command.UserId };
        var newConversation = new Conversation();

        _unitOfWorkMock.Setup(u => u.Conversations.GetPrivateConversationWithUserAsync(command.CreatorUserId, command.UserId))
            .ReturnsAsync((Conversation)null); 

        _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(command.CreatorUserId))
            .ReturnsAsync(creatorUser); 

        _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(command.UserId))
            .ReturnsAsync(targetUser); 

        _unitOfWorkMock.Setup(u => u.Conversations.CreateConversationWithUserAsync(creatorUser, targetUser))
            .ReturnsAsync(newConversation);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        ConversationDto conversationDto = _mapper.Map<ConversationDto>(newConversation);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(HttpStatusCode.Created, result.HttpStatusCode);
        Assert.AreEqual(conversationDto.Id, result.Payload.Id);
    }
}
