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
public class AddParticipantToConversationCommandTests
{
    private IMapper _mapper;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IHubService> _hubServiceMock;
    private AddParticipantToConversationCommandHandler _handler;

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
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenSomeUsersDoNotExist()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(new List<User> { });

        var missingUserIds = command.UserIds.ToList();

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync((IEnumerable<User>)new List<User> { /* existing users */ });

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
    public async Task Handle_ShouldRetrieveConversation_WhenConversationIdIsValid()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId), Times.Once);

    }

    [Test]
    public async Task Handle_ShouldRetrieveUsersByIds_WhenUserIdsAreValid()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        _unitOfWorkMock.Verify(u => u.Users.GetUsersByIdsAsync(command.UserIds), Times.Once);

    }
    [Test]
    public async Task Handle_ShouldRetrieveExistingParticipants_WhenConversationIsValid()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        _unitOfWorkMock.Verify(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldAddParticipantsToConversation_WhenParticipantsAreValid()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        _unitOfWorkMock.Verify(u => u.Participants.AddParticipantsToConversationAsync(users, conversation), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldRetrieveUserConnections_WhenUserIdsAreValid()
    {
        // Arrange
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert

        _unitOfWorkMock.Verify(u => u.Connections.GetUsersConnectionsAsync(command.UserIds), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldAddMessageToConversation_WhenAddingParticipant()
    {
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        _unitOfWorkMock.Verify(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true), Times.Once);

    }

    [Test]
    public async Task Handle_ShouldSaveChanges_WhenParticipantsAreAdded()
    {
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

    }
    [Test]
    public async Task Handle_ShouldNotifyGroup_WhenParticipantsAreAdded()
    {
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        _hubServiceMock.Verify(h => h.NotifyGroupAsync(
            It.Is<Guid>(id => id == conversation.Id),
            It.Is<MessageWithSenderDto>(msg => msg.Id == mappedJoinMessage.Id),
            It.Is<string>(method => method == "ReceiveNotification")),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldNotifyUsersConnections_WhenParticipantsAreAdded()
    {
        var command = new AddParticipantToConversationCommand
        {
            ConversationId = Guid.NewGuid(),
            UserIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId,
            Group = new Group { Title = "Test Group" }
        };

        var users = new List<User>
        {
            new User { Id = command.UserIds[0], UserName = "User1" },
            new User { Id = command.UserIds[1], UserName = "User2" }
        };

        var existingParticipants = new List<ParticipantInConversation>();

        var participants = users.Select(u => new ParticipantInConversation { User = u }).ToList();

        var userConnections = users.Select(u => new UserConnection { User = new User { Id = u.Id } }).ToList();

        var joinMessage = new Message()
        {
            Id = Guid.NewGuid(),
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetGroupConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUsersByIdsAsync(command.UserIds))
            .ReturnsAsync(users);

        _unitOfWorkMock.Setup(u => u.Participants.GetParticipantsByConversationIdAsync(command.ConversationId))
            .ReturnsAsync(existingParticipants);

        _unitOfWorkMock.Setup(u => u.Participants.AddParticipantsToConversationAsync(users, conversation))
            .ReturnsAsync(participants);

        _unitOfWorkMock.Setup(u => u.Connections.GetUsersConnectionsAsync(command.UserIds))
            .ReturnsAsync(userConnections);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(It.IsAny<string>(), conversation, null, true))
            .ReturnsAsync(joinMessage);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var joinMessageDto = new MessageDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        _hubServiceMock.Setup(h => h.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        _hubServiceMock.Verify(h => h.NotifyUsersConnectionsAsync(
            It.Is<IEnumerable<UserConnection>>(connections =>
                connections.Select(c => c.User.Id).SequenceEqual(userConnections.Select(uc => uc.User.Id))),
            It.Is<MessageDto>(msgDto => msgDto.Text == joinMessageDto.Text),
            It.Is<string>(method => method == "JoinNotification")),
            Times.Once);
    }


}
