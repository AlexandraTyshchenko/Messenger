using Messenger.Business.Options;
using Messenger.Business.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messenger.Business.Workers;

public class MetricsAggregator : BackgroundService
{
    private readonly QueueMetricsService _metrics;
    private readonly MessageQueue _queue;
    private readonly ILogger<MetricsAggregator> _logger;
    private readonly WorkerSettings _settings;

    public MetricsAggregator(
        QueueMetricsService metrics,
        MessageQueue queue,
        ILogger<MetricsAggregator> logger,
        IOptions<WorkerSettings> options)
    {
        _metrics = metrics;
        _queue = queue;
        _logger = logger;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);

            var lambda = _metrics.Lambda();
            var mu = _metrics.Mu();

            var queueLength = _queue.QueueLength();
            var inProcessing = _metrics.InProcessing();

            var isActive =
             lambda > 0 ||
             _metrics.AvgTotalTime() > 0 ||
             inProcessing > 0;

            if (!isActive)
                continue;

            var Lraw = queueLength + inProcessing;

            _metrics.AddLSample(Lraw);
            var L = _metrics.AvgL();

            var W = _metrics.AvgTotalTime();
            var Wq = _metrics.AvgWaitTime();

            double rho = 0;
            if (mu > 0)
                rho = lambda / (_settings.WorkerCount * mu);

            var lambdaW = lambda * W;

            _logger.LogInformation(
                "METRICS | L={L:F3} | Lraw={Lraw} | Lambda={Lambda:F3} | Mu={Mu:F3} | Rho={Rho:F3} | W={W:F3} | lambdaW={LW:F3}",
                L,
                Lraw,
                lambda,
                mu,
                rho,
                W,
                lambdaW);
        }
    }
}