using Messenger.Business.Enums;
using Messenger.Business.Interfaces;

namespace Messenger.Business.EventBus;

public class EventMessage
{
    public IEvent Payload { get; set; }
    public double? Lambda { get; set; }
    public ExecutionMode Mode { get; set; }
}
