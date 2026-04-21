using AutoMapper;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Queues;
using Messenger.Client.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messenger.Business.Services;

public class RealProcessor : IMessageProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RealProcessor> _logger;

    public RealProcessor(IServiceScopeFactory scopeFactory, ILogger<RealProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ProcessAsync(ChatNotification notification, CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();

        var hub = scope.ServiceProvider.GetRequiredService<IHubService>();
        var db = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var imageClient = scope.ServiceProvider.GetRequiredService<IImageClient>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        try
        {
            var sender = await db.Users.GetUserByIdAsync(notification.SenderId);
            var conversation = await db.Conversations.GetConversationByIdAsync(notification.ConversationId);

            if (sender == null || conversation == null)
            {
                _logger.LogWarning("Sender or conversation not found");
                return;
            }

            Image image = null;

            if (notification.Message.Image != null)
            {
                var response = await imageClient.UploadImageAsync(notification.Message.Image, notification.ConversationId);

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
                Text = notification.Message.Text,
                SentAt = DateTime.UtcNow
            };

            var savedMessage = await db.Messages.AddMessageToConversationAsync(message);
            await db.SaveChangesAsync();

            var mapped = mapper.Map<MessageWithSenderDto>(savedMessage);

            await hub.NotifyGroupAsync(
                notification.ConversationId,
                mapped,
                "ReceiveNotification");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }
}