using Messenger.Business.Dtos;
using Messenger.Business.EventBus;
using Messenger.Business.Interfaces;
using Messenger.Business.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messenger.Business.Services;

public class TheoreticalProcessor : IMessageProcessor
{
    private readonly WorkerSettings _settings;
    private readonly IServiceScopeFactory _scopeFactory;

    public TheoreticalProcessor(IOptions<WorkerSettings> options, IServiceScopeFactory scopeFactory)
    {
        _settings = options.Value;
        _scopeFactory = scopeFactory;
    }

    public async Task ProcessAsync(MessageSentEvent notification, CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var hub = scope.ServiceProvider.GetRequiredService<IHubService>();

        var messageWithSenderDto = new MessageWithSenderDto
        {
            Id = Guid.NewGuid(),
            ConversationId = notification.ConversationId,
            Text = notification.Message.Text,
            IsJoinMessage = notification.Message.IsJoinMessage,
            SentAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Sender =  new UserBasicInfoDto
            {
                Id = notification.SenderId,
                FirstName = "Spammer",
                LastName = "Bot"
            },
        };

        await hub.NotifyGroupAsync(
            notification.ConversationId,
            messageWithSenderDto,
            "ReceiveNotification");

        await Task.Delay(_settings.DelayMs, token);
    }
}
