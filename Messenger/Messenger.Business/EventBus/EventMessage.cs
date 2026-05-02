using Messenger.Business.Dtos;
using Messenger.Business.Enums;

namespace Messenger.Business.EventBus;

public class EventMessage : IEvent
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public MessageDto Message { get; set; }

    public double? Lambda { get; set; }
    public double? Mu { get; set; }

    public ExecutionMode Mode { get; set; }
}