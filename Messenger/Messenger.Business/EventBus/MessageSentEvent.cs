using Messenger.Business.Dtos;

namespace Messenger.Business.EventBus;

public class MessageSentEvent : IEvent
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }

    public MessageDto Message { get; set; }

    public DateTime StartProcessingTime { get; set; }

    public DateTime ArrivalTime { get; set; }
}
