using AutoMapper;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Profiles;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Moq;
using System.Collections.Generic;
using System.Net;

namespace Messenger.Business.Tests;

[TestFixture]
public class AddParticipantToConversationCommandTests
{
    private IMapper _mapper;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IHubService> _hubServiceMock;
    private AddParticipantToConversationCommandHandler _handler;

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
        _handler = new AddParticipantToConversationCommandHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _hubServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenConversationDoesNotExist()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid() }
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync((Conversation)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Group conversation was not found.", result.ErrorMessage);

        _hubServiceMock.Verify(h => h.NotifyGroupAsync(It.IsAny<Guid>(), It.IsAny<MessageWithSenderDto>(),
            It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenSomeUsersDoNotExist()
    {
        // Arrange
        var existingUserId = Guid.NewGuid();
        var missingUserId = Guid.NewGuid();

        var existingUser = new User
        {
            Id = existingUserId,
        };

        var missingUser = new User
        {
            Id = missingUserId,
        };

        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { existingUserId, missingUserId }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(new List<User> { existingUser });

        var missingUserIds = new Guid[] { missingUserId }; 

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.IsFalse(result.Success);
        Assert.AreEqual($"Users with ids {string.Join(", ", missingUserIds)} were not found.", result.ErrorMessage);
    }


    [Test]
    public async Task Handle_ShouldReturnConflict_WhenUsersAreAlreadyParticipants()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId
        };

        var existingUser = new User { Id = command.UserIds.First() };
        var existingParticipant = new ParticipantInConversation { User = existingUser };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(new List<User> { existingUser });

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(new List<ParticipantInConversation> { existingParticipant });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(HttpStatusCode.Conflict, result.HttpStatusCode);
        Assert.IsFalse(result.Success);
        Assert.AreEqual($"Users {string.Join(", ", existingUser.UserName)} already exist in groupConversation.", result.ErrorMessage);
    }

    [Test]

    public async Task Handle_ShouldReturnCorrectParticipantsWithOkStatusCode_WhenConversationAndUsersIdsAreValid()
    {
        // Arrange
        var conversationId = Guid.NewGuid();

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var userIds = new[] { userId1, userId2 };
        var users = userIds.Select(id => new User { Id = id, UserName = $"User{id}" }).ToList();
        var conversation = new Conversation
        {
            Id = conversationId,
            Group = new Group { Title = "Test Group" }
        };
        var participants = users.Select(user => new ParticipantInConversation { User = user }).ToList();

        var joinMessage = new Message { Id = Guid.NewGuid(), Text = "Test message" };

        _unitOfWorkMock.Setup(uow => uow.Conversations.GetGroupConversationByIdAsync(conversationId))
            .ReturnsAsync(conversation);
        _unitOfWorkMock.Setup(uow => uow.Users.GetUsersByIdsAsync(userIds))
            .ReturnsAsync(users);
        _unitOfWorkMock.Setup(uow => uow.Participants.GetParticipantsByConversationIdAsync(conversationId))
            .ReturnsAsync(new List<ParticipantInConversation>()); // No existing participants
        _unitOfWorkMock.Setup(uow => uow.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);
        _unitOfWorkMock.Setup(uow => uow.Messages.AddMessageToConversationAsync(It.IsAny<Message>()))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(uow => uow.Connections.GetUsersConnectionsAsync(userIds))
            .ReturnsAsync(new List<UserConnection> { new UserConnection { User = participants.First().User } });

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedParticipants = _mapper.Map<IEnumerable<ParticipantsDto>>(participants);
        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(hub => hub.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);
        _hubServiceMock.Setup(hub => hub.NotifyUsersConnectionsAsync(It.IsAny<IEnumerable<UserConnection>>(), It.IsAny<NotificationDto>(), "JoinNotification"))
            .Returns(Task.CompletedTask);

        var command = new AddParticipantToConversationCommand
        {
            ConversationId = conversationId,
            UserIds = userIds
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.AreEqual(mappedParticipants.FirstOrDefault(x=>x.UserInfo.Id== userId1).UserInfo.Id, result.Payload.FirstOrDefault(x => x.UserInfo.Id == userId1).UserInfo.Id);
        Assert.AreEqual(mappedParticipants.Count(), result.Payload.Count());

        _hubServiceMock.Verify(hub => hub.NotifyGroupAsync(
            It.IsAny<Guid>(),
            It.Is<MessageWithSenderDto>(x => x.Text == mappedJoinMessage.Text),
            "ReceiveNotification"),
            Times.Once);
        _hubServiceMock.Verify(hub => hub.NotifyUsersConnectionsAsync(It.IsAny<IEnumerable<UserConnection>>(), It.IsAny<NotificationDto>(), "JoinNotification"), Times.Once);
    }
}
