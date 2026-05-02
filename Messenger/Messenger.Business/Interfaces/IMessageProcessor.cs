using Messenger.Business.EventBus;

namespace Messenger.Business.Interfaces;

public interface IMessageProcessor
{
    Task ProcessAsync(EventMessage message, CancellationToken token);
}