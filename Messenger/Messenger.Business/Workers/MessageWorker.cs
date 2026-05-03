using Messenger.Business.Dispatchers;
using Messenger.Business.Enums;
using Messenger.Business.EventBus;
using Messenger.Business.Options;
using Messenger.Business.Queues;
using Messenger.Business.Services;
using Messenger.Infrastructure.Entities;
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

            var queueLength = _queue.QueueLength();
            _metrics.StartProcessing(queueLength);

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

                var totalTime = (DateTime.UtcNow - queueItem.ArrivalTime).TotalSeconds;
                var waitTime = (queueItem.StartProcessingTime - queueItem.ArrivalTime).TotalSeconds;

                _metrics.AddTimes(totalTime, waitTime, sw.Elapsed.TotalSeconds);

                LogMetrics(id, queueItem.Message);
                var queueLengthAfter = _queue.QueueLength();

                _metrics.EndProcessing(queueLengthAfter);

            }
        }
    }

    private void LogMetrics(int id, EventMessage message)
    {
        var lambda = _metrics.Lambda();
        var mu = _metrics.Mu();
        var rho = _metrics.Rho();

        var W = _metrics.W();
        var Wq = _metrics.Wq();
        var L = _metrics.L();
        var Lq = _metrics.Lq();

        var littleL = lambda * W;
        var littleLq = lambda * Wq;

        var queueLength = _queue.QueueLength();
        var inProcessing = _metrics.InProcessing();
        var L_real = queueLength + inProcessing;

        var isTheoretical = message.Mode == ExecutionMode.Theoretical;

        var mode = isTheoretical ? "THEORY" : "REAL";

        string tag = isTheoretical
            ? $"THEORY_lambda={(message.Lambda?.ToString("F2") ?? "N/A")}_c={_settings.WorkerCount}_d={(message.Mu?.ToString("F2") ?? "N/A")}"
            : $"REAL_lambda={(message.Lambda?.ToString("F2") ?? "N/A")}_c={_settings.WorkerCount}";

        _logger.LogInformation(
            "Mode={Mode} | Tag={Tag} | Worker={WorkerId} | " +
            "L={L:F3} | lambdaW={LittleL:F3} | Lq={Lq:F3} | lambdaWq={LittleLq:F3} | " +
            "W={W:F3} | Wq={Wq:F3} | " +
            "lambda={Lambda:F3} | mu={Mu:F3} | p={Rho:F3} | " +
            "Queue={QueueLength} | InProc={InProcessing} | L_inst={LReal}",
            mode,
            tag,
            id,
            L,
            littleL,
            Lq,
            littleLq,
            W,
            Wq,
            lambda,
            mu,
            rho,
            queueLength,
            inProcessing,
            L_real);
    }
}