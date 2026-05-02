namespace Messenger.Business.EventBus;

public interface IEventPublisher
{
    Task PublishAsync(EventMessage message, CancellationToken token = default);
}
