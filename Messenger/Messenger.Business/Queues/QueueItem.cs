using Messenger.Business.EventBus;

namespace Messenger.Business.Queues;

public class QueueItem
{
    public EventMessage Message { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime StartProcessingTime { get; set; }
}