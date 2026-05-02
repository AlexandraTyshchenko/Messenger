using Messenger.Business.EventBus;
using Messenger.Business.Interfaces;
using Messenger.Business.Services;
using System.Threading;
using System.Threading.Channels;

namespace Messenger.Business.Queues;

public class MessageQueue
{
    private readonly Channel<QueueItem> _channel;
    private readonly QueueMetricsService _metrics;
    private int _queueLength = 0;

    public MessageQueue(QueueMetricsService metrics)
    {
        _metrics = metrics;
        _channel = Channel.CreateUnbounded<QueueItem>();
    }

    public async ValueTask EnqueueAsync(EventMessage message, CancellationToken cancellationToken)
    {
        var item = new QueueItem
        {
            Message = message,
            ArrivalTime = DateTime.UtcNow
        };

        _metrics.MessageReceived();
        Interlocked.Increment(ref _queueLength);

        await _channel.Writer.WriteAsync(item, cancellationToken);
    }

    public async ValueTask<QueueItem> DequeueAsync(CancellationToken token)
    {
        var item = await _channel.Reader.ReadAsync(token);

        Interlocked.Decrement(ref _queueLength);

        item.StartProcessingTime = DateTime.UtcNow;

        return item;
    }

    public int QueueLength() => Volatile.Read(ref _queueLength);
}