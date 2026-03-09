using Messenger.Business.Services;
using System.Threading.Channels;

namespace Messenger.Business.Queues;

public class MessageQueue
{
    private readonly Channel<ChatNotification> _channel;
    private readonly QueueMetricsService _metrics;

    public MessageQueue(QueueMetricsService metrics)
    {
        _metrics = metrics;
        _channel = Channel.CreateUnbounded<ChatNotification>();
    }

    public async ValueTask EnqueueAsync(ChatNotification notification)
    {
        _metrics.MessageReceived();

        await _channel.Writer.WriteAsync(notification);
    }

    public ValueTask<ChatNotification> DequeueAsync(CancellationToken token)
    {
        return _channel.Reader.ReadAsync(token);
    }
}