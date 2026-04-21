using Messenger.Business.Services;
using System.Threading.Channels;

namespace Messenger.Business.Queues;

public class MessageQueue
{
    private readonly Channel<ChatNotification> _channel;
    private readonly QueueMetricsService _metrics;

    private int _queueLength = 0;

    public MessageQueue(QueueMetricsService metrics)
    {
        _metrics = metrics;
        _channel = Channel.CreateUnbounded<ChatNotification>();
    }

    public async ValueTask EnqueueAsync(ChatNotification notification, CancellationToken cancellationToken = default)
    {
        _metrics.MessageReceived();
        Interlocked.Increment(ref _queueLength);
        notification.ArrivalTime = DateTime.UtcNow;

        await _channel.Writer.WriteAsync(notification, cancellationToken);
    }

    public async ValueTask<ChatNotification> DequeueAsync(CancellationToken token)
    {
        var notification = await _channel.Reader.ReadAsync(token);
        Interlocked.Decrement(ref _queueLength);
        notification.StartProcessingTime = DateTime.UtcNow;
        return notification;
    }

    public int QueueLength()
    {
        return _queueLength;
    }
}