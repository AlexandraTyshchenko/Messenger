using Messenger.Business.Queues;
using Messenger.Infrastructure.Entities;
using Newtonsoft.Json.Linq;

namespace Messenger.Business.EventBus;

public class InMemoryEventPublisher : IEventPublisher
{
    private readonly MessageQueue _queue;

    public InMemoryEventPublisher(MessageQueue queue)
    {
        _queue = queue;
    }


    public async Task PublishAsync(EventMessage eventMessage, CancellationToken cancellationToken)
    {
        await _queue.EnqueueAsync(eventMessage, cancellationToken);
    }
}