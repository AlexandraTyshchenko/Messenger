using AutoMapper;
using Messenger.Business.Dtos;
using Messenger.Business.EventBus;
using Messenger.Business.Interfaces;
using Messenger.Client.Interfaces;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Messenger.Business.Services;

public class RealProcessor : IMessageProcessor
{
    private readonly IHubService _hub;
    private readonly IUnitOfWork _db;
    private readonly IImageClient _imageClient;
    private readonly IMapper _mapper;
    private readonly ILogger<RealProcessor> _logger;

    public RealProcessor(
        IHubService hub,
        IUnitOfWork db,
        IImageClient imageClient,
        IMapper mapper,
        ILogger<RealProcessor> logger)
    {
        _hub = hub;
        _db = db;
        _imageClient = imageClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task ProcessAsync(EventMessage eventMessage, CancellationToken token)
    {
        try
        {
            var sender = await _db.Users.GetUserByIdAsync(eventMessage.SenderId);
            var conversation = await _db.Conversations.GetConversationByIdAsync(eventMessage.ConversationId);

            if (sender == null || conversation == null)
            {
                _logger.LogWarning("Sender or conversation not found");
                return;
            }

            Image image = null;

            if (eventMessage.Message.Image != null)
            {
                var response = await _imageClient.UploadImageAsync(
                    eventMessage.Message.Image,
                    eventMessage.ConversationId);

                if (!response.Success)
                {
                    _logger.LogWarning("Image upload failed");
                    return;
                }

                image = new Image
                {
                    ImageUrl = response.Payload.RelativePath,
                    FileName = response.Payload.FileName
                };
            }

            var message = new Message
            {
                Image = image,
                Conversation = conversation,
                Sender = sender,
                Text = eventMessage.Message.Text,
                SentAt = DateTime.UtcNow
            };

            var savedMessage = await _db.Messages.AddMessageToConversationAsync(message);
            await _db.SaveChangesAsync();

            var mapped = _mapper.Map<MessageWithSenderDto>(savedMessage);

            await _hub.NotifyGroupAsync(
                eventMessage.ConversationId,
                mapped,
                "ReceiveNotification");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }
}