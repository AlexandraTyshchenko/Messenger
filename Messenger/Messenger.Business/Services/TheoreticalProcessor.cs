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
    private static readonly Random _random = new();

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

        var delayMs = GetExponentialDelayMs(_settings.Mu);
        await Task.Delay(delayMs, token);
    }


    private int GetExponentialDelayMs(double mu)
    {
        var u = _random.NextDouble();
        var delaySeconds = -Math.Log(u) / mu;
        return (int)(delaySeconds * 1000);
    }
}
