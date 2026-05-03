using Messenger.Business.Enums;
using Messenger.Business.Options;
using Messenger.Business.Queues;
using Messenger.Business.Services;
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
                 lambda > 0 &&
                 mu > 0 &&
                 (queueLength > 0 || inProcessing > 0);
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

            var lambdaInput = _metrics.LambdaInput();
            var muInput = _metrics.MuInput();
            var mode = _metrics.ModeInput();

            // ======================
            // THEORY
            // ======================
            double? LTheory = null;
            double? WTheory = null;
            double? WqTheory = null;
            double? rhoTheory = null;

            if (lambdaInput.HasValue && muInput.HasValue && muInput > 0)
            {
                var l = lambdaInput.Value;
                var m = muInput.Value;

                var rhoTh = l / m;
                rhoTheory = rhoTh;

                if (rhoTh < 1)
                {
                    LTheory = rhoTh / (1 - rhoTh);
                    WTheory = 1.0 / (m - l);
                    WqTheory = WTheory - (1.0 / m);
                }
            }

            // ======================
            // ERRORS
            // ======================
            double? errL = null;
            double? errW = null;
            double? errWq = null;

            if (LTheory.HasValue && LTheory > 0)
                errL = Math.Abs(L - LTheory.Value) / LTheory.Value;

            if (WTheory.HasValue && WTheory > 0)
                errW = Math.Abs(W - WTheory.Value) / WTheory.Value;

            if (WqTheory.HasValue && WqTheory > 0)
                errWq = Math.Abs(Wq - WqTheory.Value) / WqTheory.Value;

            // ======================
            // SMOOTH ERRORS
            // ======================
            double? errLSm = null;
            double? errWSm = null;
            double? errWqSm = null;

            if (errL.HasValue)
                errLSm = _metrics.SmoothErrL(errL.Value);

            if (errW.HasValue)
                errWSm = _metrics.SmoothErrW(errW.Value);

            if (errWq.HasValue)
                errWqSm = _metrics.SmoothErrWq(errWq.Value);

            // ======================
            // LOG
            // ======================
            var modeStr = mode == ExecutionMode.Theoretical ? "Theoretical" : "Real";
            var tag = $"MODE={modeStr}_lambda={lambdaInput:F2}_mu={muInput:F2}_c={_settings.WorkerCount}";

            _logger.LogInformation(
                "TAG={Tag} | " +
                "L={L} | Lth={Lth} | ErrL={ErrL} | ErrLsm={ErrLsm} | " +
                "W={W} | Wth={Wth} | ErrW={ErrW} | ErrWsm={ErrWsm} | " +
                "Wq={Wq} | WqTh={WqTh} | ErrWq={ErrWq} | ErrWqsm={ErrWqsm} | " +
                "LambdaReal={Lambda} | MuReal={Mu} | Rho={Rho}",

                tag,

                Math.Round(L, 3),
                LTheory.HasValue ? Math.Round(LTheory.Value, 3) : (double?)null,
                errL,
                errLSm,

                Math.Round(W, 3),
                WTheory.HasValue ? Math.Round(WTheory.Value, 3) : (double?)null,
                errW,
                errWSm,

                Math.Round(Wq, 3),
                WqTheory.HasValue ? Math.Round(WqTheory.Value, 3) : (double?)null,
                errWq,
                errWqSm,

                Math.Round(lambda, 3),
                Math.Round(mu, 3),
                Math.Round(rho, 3)
            );
        }
    }
}