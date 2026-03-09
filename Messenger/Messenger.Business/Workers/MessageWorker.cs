using Messenger.Business.Interfaces;
using Messenger.Business.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Messenger.Business.Services;

namespace Messenger.Business.Workers;

public class MessageWorker : BackgroundService
{
    private readonly MessageQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MessageWorker> _logger;
    private readonly DateTime _start = DateTime.UtcNow;
    private readonly QueueMetricsService _metrics;

    public MessageWorker(MessageQueue queue, IServiceScopeFactory scopeFactory, ILogger<MessageWorker> logger, QueueMetricsService metrics)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _metrics = metrics;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MessageWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var notification = await _queue.DequeueAsync(stoppingToken);

            using var scope = _scopeFactory.CreateScope();

            var hubService = scope.ServiceProvider
                .GetRequiredService<IHubService>();

            _metrics.MessageProcessed();

            var lambda = _metrics.Lambda();
            var mu = _metrics.Mu();
            var rho = _metrics.Rho();
            var overloaded = rho > 1;
            var queueLength = _queue.QueueLength();

            _logger.LogInformation(
               "SYSTEM METRICS → QueueLength: {QueueLength} | ArrivalRate: {Lambda:F2} msg/sec | ServiceRate: {Mu:F2} msg/sec | ServerUtilization: {Rho:F2} | ServerOverloaded: {Overloaded}",
               queueLength,
               lambda,
               mu,
               rho,
               overloaded);

            await hubService.NotifyGroupAsync(
                notification.ConversationId,
                notification.Message,
                "ReceiveNotification");

            await Task.Delay(5000);
        }
    }
}