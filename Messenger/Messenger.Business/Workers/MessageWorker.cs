using Messenger.Business.Interfaces;
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
            var notification = await _queue.DequeueAsync(token);

            _metrics.StartProcessing();
            var sw = Stopwatch.StartNew();

            _logger.LogInformation("Worker {Id} START", id);

            try
            {
                await ProcessNotification(notification, token);
            }
            finally
            {
                sw.Stop();

                LogMetrics(id, notification, sw.Elapsed.TotalSeconds);

                _metrics.EndProcessing();

                _logger.LogInformation("Worker {Id} END", id);
            }
        }
    }

    private async Task ProcessNotification(ChatNotification notification, CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();

        var hubService = scope.ServiceProvider
            .GetRequiredService<IHubService>();

        await hubService.NotifyGroupAsync(
            notification.ConversationId,
            notification.Message,
            "ReceiveNotification");

        if (_settings.DelayMs > 0)
        {
            await Task.Delay(_settings.DelayMs, token);
        }
    }

    private void LogMetrics(int id, ChatNotification notification, double serviceTime)
    {
        var totalTime = (DateTime.UtcNow - notification.ArrivalTime).TotalSeconds;
        var queueWaitTime = (notification.StartProcessingTime - notification.ArrivalTime).TotalSeconds;

        _metrics.AddServiceTime(serviceTime);
        _metrics.MessageProcessed();

        var inProcessing = _metrics.InProcessing();
        var lambda = _metrics.Lambda();
        var mu = _metrics.MuReal();
        var rho = _metrics.Rho();
        var queueLength = _queue.QueueLength();

        var L_real = queueLength + inProcessing;

        _logger.LogInformation(
            "Worker {Id} | QueueLength: {QueueLength} | InProcessing: {InProcessing} | L(real): {Lreal} | W: {W:F2} | Wq: {Wq:F2} | S: {S:F2} | Lambda: {Lambda:F2} | μ: {Mu:F2} | p: {Rho:F2}",
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