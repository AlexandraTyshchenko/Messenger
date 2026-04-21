using Messenger.Business.Queues;

namespace Messenger.Business.Interfaces;

public interface IMessageProcessor
{
    Task ProcessAsync(ChatNotification notification, CancellationToken token);
}