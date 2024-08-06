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
public class AddMessageToConversationCommandHandlerTests
{
    private IMapper _mapper;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IHubService> _hubServiceMock;
    private AddMessageToConversationCommandHandler _handler;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [SetUp]//todo read
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _hubServiceMock = new Mock<IHubService>();
        _handler = new AddMessageToConversationCommandHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _hubServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenConversationNotFound()
    {
        // Arrange
        var command = new AddMessageToConversationCommand
        {
            SenderId = Guid.NewGuid(),
            ConversationId = Guid.NewGuid(),
            Message = new MessageDto { Text = "Hello" }
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Conversation)null);

        _unitOfWorkMock.Setup(r => r.Users.GetUserByIdAsync(command.SenderId))
         .ReturnsAsync(new User
         {
             Id = command.SenderId,
         });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.IsFalse(result.Success);
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenSenderNotFound()
    {
        // Arrange
        var command = new AddMessageToConversationCommand
        {
            SenderId = Guid.NewGuid(),
            ConversationId = Guid.NewGuid(),
            Message = new MessageDto { Text = "Hello" }
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Conversation());

        _unitOfWorkMock.Setup(r => r.Users.GetUserByIdAsync(command.SenderId))
         .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.IsFalse(result.Success);
    }

    [Test]
    public async Task Handle_ShouldReturnCorrectMessageWithOkStatusCode_WhenConversationIsFound()
    {
        // Arrange
        var command = new AddMessageToConversationCommand
        {
            SenderId = Guid.NewGuid(),
            ConversationId = Guid.NewGuid(),
            Message = new MessageDto { Text = "Hello" }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId
        };

        var sender = new User
        {
            Id = command.SenderId
        };

        var message = new Message
        {
            Text = command.Message.Text,
            Sender = sender,
            Conversation = conversation
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(command.SenderId))
            .ReturnsAsync(sender);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(command.Message.Text, conversation, sender, false))
            .ReturnsAsync(message);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, It.IsAny<MessageWithSenderDto>(), "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(message);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(HttpStatusCode.Created, result.HttpStatusCode);
        Assert.AreEqual(mappedMessage.Id, result.Payload.Id);
        Assert.AreEqual(mappedMessage.SentAt, result.Payload.SentAt);
        Assert.AreEqual(mappedMessage.Sender.Id, result.Payload.Sender.Id);
    }

    [Test]
    public async Task Handle_ShouldCallNotifyGroupAsync_WhenMessageIsAddedToConversation()
    {
        // Arrange
        var command = new AddMessageToConversationCommand
        {
            SenderId = Guid.NewGuid(),
            ConversationId = Guid.NewGuid(),
            Message = new MessageDto { Text = "Hello" }
        };

        var conversation = new Conversation
        {
            Id = command.ConversationId
        };

        var sender = new User
        {
            Id = command.SenderId
        };

        var message = new Message
        {
            Text = command.Message.Text,
            Sender = sender,
            Conversation = conversation
        };

        _unitOfWorkMock.Setup(u => u.Conversations.GetConversationByIdAsync(command.ConversationId))
            .ReturnsAsync(conversation);

        _unitOfWorkMock.Setup(u => u.Users.GetUserByIdAsync(command.SenderId))
            .ReturnsAsync(sender);

        _unitOfWorkMock.Setup(u => u.Messages.AddMessageToConversationAsync(command.Message.Text, conversation, sender, false))
            .ReturnsAsync(message);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(message);

        _hubServiceMock.Setup(h => h.NotifyGroupAsync(conversation.Id, mappedMessage, "ReceiveNotification"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _hubServiceMock.Verify(h => h.NotifyGroupAsync(
            conversation.Id,
            It.Is<MessageWithSenderDto>(m => m.Text == mappedMessage.Text && m.Sender.Id == mappedMessage.Sender.Id),
            "ReceiveNotification"),
        Times.Once);
    }
}