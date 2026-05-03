using Messenger.Business.EventBus;
using Messenger.Business.Services;
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

    public async Task EnqueueAsync(EventMessage eventMessage, CancellationToken cancellationToken)
    {
        var item = new QueueItem
        {
            Message = eventMessage,
            ArrivalTime = DateTime.UtcNow
        };
        _metrics.MessageReceived(_queueLength);

        Interlocked.Increment(ref _queueLength);

        // ВАЖНО: передаём длину очереди

        await _channel.Writer.WriteAsync(item, cancellationToken);
    }

    public async ValueTask<QueueItem> DequeueAsync(CancellationToken token)
    {
        var item = await _channel.Reader.ReadAsync(token);

        item.StartProcessingTime = DateTime.UtcNow;

        Interlocked.Decrement(ref _queueLength);

        return item;
    }

    public int QueueLength() => Volatile.Read(ref _queueLength);
}