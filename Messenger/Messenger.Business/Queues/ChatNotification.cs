using Messenger.Business.Dtos;

namespace Messenger.Business.Queues;

public class ChatNotification
{
    public Guid ConversationId { get; set; }

    public MessageWithSenderDto Message { get; set; }

    public DateTime StartProcessingTime { get; set; }

    public DateTime ArrivalTime { get; set; }
}
