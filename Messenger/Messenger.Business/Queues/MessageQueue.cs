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

    public async ValueTask EnqueueAsync(ChatNotification notification)
    {
        _metrics.MessageReceived();

        Interlocked.Increment(ref _queueLength);

        await _channel.Writer.WriteAsync(notification);
    }

    public async ValueTask<ChatNotification> DequeueAsync(CancellationToken token)
    {
        var item = await _channel.Reader.ReadAsync(token);

        Interlocked.Decrement(ref _queueLength);

        return item;
    }

    public int QueueLength()
    {
        return _queueLength;
    }
}