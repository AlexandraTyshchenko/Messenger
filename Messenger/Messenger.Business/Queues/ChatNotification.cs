using Messenger.Business.Dtos;

namespace Messenger.Business.Queues;

public class ChatNotification
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }

    public MessageDto Message { get; set; }

    public DateTime StartProcessingTime { get; set; }

    public DateTime ArrivalTime { get; set; }
    public bool IsTheoretical { get; set; }
    public double? TheoreticalLambda { get; set; }
}
