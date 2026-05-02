using Messenger.Business.Dispatchers;
using Messenger.Business.Enums;
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
        _logger.LogInformation("MessageWorker started with {Count} workers", _settings.WorkerCount);

        var tasks = Enumerable.Range(0, _settings.WorkerCount)
            .Select(i => Task.Run(() => ProcessLoop
            (i, stoppingToken), stoppingToken));

        await Task.WhenAll(tasks);
    }

    private async Task ProcessLoop(int id, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var queueItem = await _queue.DequeueAsync(token);

            _metrics.StartProcessing();
            var sw = Stopwatch.StartNew();

            _logger.LogInformation("Worker {Id} START", id);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<EventDispatcher>();
                await dispatcher.DispatchAsync(queueItem.Message, token);
            }
            finally
            {
                sw.Stop();

                LogMetrics(id, queueItem, sw.Elapsed.TotalSeconds);

                _metrics.EndProcessing();

                _logger.LogInformation("Worker {Id} END", id);
            }
        }
    }


    private void LogMetrics(int id, QueueItem queueItem, double serviceTime)
    {
        var message = queueItem.Message;

        var totalTime = (DateTime.UtcNow - queueItem.ArrivalTime).TotalSeconds;
        var queueWaitTime = (queueItem.StartProcessingTime - queueItem.ArrivalTime).TotalSeconds;

        _metrics.AddServiceTime(serviceTime);
        _metrics.MessageProcessed();

        var inProcessing = _metrics.InProcessing();
        var lambda = _metrics.Lambda();
        var mu = _metrics.MuReal();
        var rho = _metrics.Rho();
        var queueLength = _queue.QueueLength();

        var L_real = queueLength + inProcessing;

        var isTheoretical = message.Mode == ExecutionMode.Theoretical;

        string tag = isTheoretical
           ? $"THEORY_lambda={(message.Lambda?.ToString("F2") ?? "N/A")}_c={_settings.WorkerCount}_d={_settings.Mu}"
           : $"REAL_lambda={(message.Lambda?.ToString("F2") ?? "N/A")}_c={_settings.WorkerCount}";

        _logger.LogInformation(
            "TAG={Tag} | MODE={Mode} | Workers={Workers} | WorkerId={WorkerId} | QueueLength={QueueLength} | InProcessing={InProcessing} | LReal={LReal} | W={W:F3} | Wq={Wq:F3} | S={S:F3} | Lambda={Lambda:F3} | Mu={Mu:F3} | Rho={Rho:F3}",
            tag,
            isTheoretical ? "THEORETICAL" : "REAL",
            _settings.WorkerCount,
            id,
            queueLength,
            inProcessing,
            L_real,
            totalTime,
            queueWaitTime,
            serviceTime,
            lambda,
            mu,
            rho);
    }
}