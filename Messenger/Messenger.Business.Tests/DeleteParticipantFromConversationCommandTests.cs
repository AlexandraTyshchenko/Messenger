using AutoMapper;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Profiles;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Moq;
using System.Net;

namespace Messenger.Business.Tests;

[TestFixture]
public class DeleteParticipantFromConversationCommandTests
{
    private IMapper _mapper;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IHubService> _hubServiceMock;
    private DeleteParticipantFromConversationCommandHandler _handler;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _hubServiceMock = new Mock<IHubService>();
        _handler = new DeleteParticipantFromConversationCommandHandler(
            _unitOfWorkMock.Object,
            _hubServiceMock.Object,
            _mapper
        );
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenConversationDoesNotExist()
    {
        // Arrange
        var command = new DeleteParticipantFromConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(command.ConversationId))
            .ReturnsAsync((Conversation)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.AreEqual($"Conversation with id {command.ConversationId} wasn't found.", result.ErrorMessage);
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenDeletingParticipantLeavesConversationEmpty()
    {
        // Arrange
        var command = new DeleteParticipantFromConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(new List<ParticipantInConversation> { /* single participant */ });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.AreEqual("Cannot delete participant because it would leave the conversation with zero participants.", result.ErrorMessage);
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenParticipantIsNotInConversation()
    {
        // Arrange
        var command = new DeleteParticipantFromConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(new List<ParticipantInConversation>() {
                new ParticipantInConversation(),
                new ParticipantInConversation()
            });

        _unitOfWorkMock.Setup(u => u.Participants.DeleteParticipantFromGroupConversationAsync(command.UserId, command.ConversationId))
                .ReturnsAsync((ParticipantInConversation)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.AreEqual($"User with id {command.UserId} wasn't found in group conversation with id {command.ConversationId}.", result.ErrorMessage);
    }

    [Test]
    public async Task Handle_ShouldDeleteParticipant_WhenValidRequest()
    {
        // Arrange
        var command = new DeleteParticipantFromConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var participantInConversation = new ParticipantInConversation
        {
            Id = command.UserId,
            Role = Role.Participant,
            Conversation = conversation,
            User = new User { UserName = "user" }
        };

        var remainingParticipants = new List<ParticipantInConversation>
        {
            participantInConversation,
            new ParticipantInConversation()
        };

        var message = new Message
        {
            Id = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(remainingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.DeleteParticipantFromGroupConversationAsync(command.UserId, command.ConversationId))
            .ReturnsAsync(participantInConversation);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<Message>()))
            .ReturnsAsync(message);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        _unitOfWorkMock.Setup(u => u.Connections.GetUserConnectionsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<UserConnection>());

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, It.IsAny<MessageWithSenderDto>(), "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(It.IsAny<IEnumerable<UserConnection>>(), It.IsAny<NotificationDto>(), "LeaveConversationNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
        _unitOfWorkMock.Verify(u => u.Participants.DeleteParticipantFromGroupConversationAsync(command.UserId, command.ConversationId), Times.Once);
    }
}
