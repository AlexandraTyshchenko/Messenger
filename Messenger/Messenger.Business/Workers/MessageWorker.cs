using Messenger.Business.Dispatchers;
using Messenger.Business.Options;
using Messenger.Business.Queues;
using Messenger.Business.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

public class MessageWorker : BackgroundService
{
    private readonly MessageQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MessageWorker> _logger;
    private readonly QueueMetricsService _metrics;
    private readonly WorkerSettings _settings;

    public MessageWorker(
        MessageQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<MessageWorker> logger,
        QueueMetricsService metrics,
        IOptions<WorkerSettings> options)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _metrics = metrics;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = Enumerable.Range(0, _settings.WorkerCount)
            .Select(i => Task.Run(() => ProcessLoop(i, stoppingToken), stoppingToken));

        await Task.WhenAll(tasks);
    }

    private async Task ProcessLoop(int id, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var queueItem = await _queue.DequeueAsync(token);

            _metrics.StartProcessing();
            var sw = Stopwatch.StartNew();

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<EventDispatcher>();
                await dispatcher.DispatchAsync(queueItem.Message, token);
            }
            finally
            {
                sw.Stop();

                var serviceTime = sw.Elapsed.TotalSeconds;
                var totalTime = (DateTime.UtcNow - queueItem.ArrivalTime).TotalSeconds;
                var waitTime = (queueItem.StartProcessingTime - queueItem.ArrivalTime).TotalSeconds;

                _metrics.AddServiceTime(serviceTime);
                _metrics.AddTimes(totalTime, waitTime);

                _metrics.EndProcessing();
            }
        }
    }
}