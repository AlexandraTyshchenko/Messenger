using Messenger.Business.Queues;

namespace Messenger.Business.EventBus;

public class InMemoryEventPublisher : IEventPublisher
{
    private readonly MessageQueue _queue;

    public InMemoryEventPublisher(MessageQueue queue)
    {
        _queue = queue;
    }

    public async Task PublishAsync(EventMessage message, CancellationToken token = default)
    {
        await _queue.EnqueueAsync(message, token);
    }
}