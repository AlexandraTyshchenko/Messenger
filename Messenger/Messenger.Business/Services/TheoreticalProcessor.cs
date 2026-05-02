using Messenger.Business.Dtos;
using Messenger.Business.EventBus;
using Messenger.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Services;

public class TheoreticalProcessor : IMessageProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly Random _random = new();

    public TheoreticalProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ProcessAsync(EventMessage eventMessage, CancellationToken token)
    {
        var mu = eventMessage.Mu.HasValue ? eventMessage.Mu.Value : 2.0;
        
        var delayMs = GetExponentialDelayMs(mu);
        await Task.Delay(delayMs, token);

        using var scope = _scopeFactory.CreateScope();
        var hub = scope.ServiceProvider.GetRequiredService<IHubService>();

        var messageWithSenderDto = new MessageWithSenderDto
        {
            Id = Guid.NewGuid(),
            ConversationId = eventMessage.ConversationId,
            Text = eventMessage.Message.Text,
            IsJoinMessage = eventMessage.Message.IsJoinMessage,
            SentAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Sender = new UserBasicInfoDto
            {
                Id = eventMessage.SenderId,
                FirstName = "Spammer",
                LastName = "Bot"
            },
        };

        await hub.NotifyGroupAsync(
            eventMessage.ConversationId,
            messageWithSenderDto,
            "ReceiveNotification");
    }


    private int GetExponentialDelayMs(double mu)
    {
        var u = _random.NextDouble();
        var delaySeconds = -Math.Log(u) / mu;
        return (int)(delaySeconds * 1000);
    }
}
