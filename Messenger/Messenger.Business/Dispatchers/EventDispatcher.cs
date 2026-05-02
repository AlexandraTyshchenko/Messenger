using Messenger.Business.Enums;
using Messenger.Business.EventBus;
using Messenger.Business.Interfaces;
using Messenger.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Dispatchers;

public class EventDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EventDispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task DispatchAsync(EventMessage message, CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        IMessageProcessor processor = message.Mode == ExecutionMode.Theoretical
            ? scope.ServiceProvider.GetRequiredService<TheoreticalProcessor>()
            : scope.ServiceProvider.GetRequiredService<RealProcessor>();

        await processor.ProcessAsync((MessageSentEvent)message.Payload, token);
    }
}
