namespace Messenger.Business.EventBus;

public interface IEventPublisher
{
    Task PublishAsync(EventMessage eventMessage, CancellationToken cancellationToken);
}
