using Messenger.Business.Enums;
using System.Collections.Concurrent;

namespace Messenger.Business.Services;

public class QueueMetricsService
{
    private readonly ConcurrentQueue<DateTime> _arrivals = new();

    private readonly ConcurrentQueue<(DateTime time, double value)> _serviceTimes = new();
    private readonly ConcurrentQueue<(DateTime time, double value)> _totalTimes = new();
    private readonly ConcurrentQueue<(DateTime time, double value)> _waitTimes = new();
    private readonly ConcurrentQueue<(DateTime time, double value)> _lSamples = new();

    private readonly TimeSpan _window = TimeSpan.FromSeconds(40);

    private int _inProcessing = 0;

    // ======================
    // SMOOTHING λ, μ
    // ======================
    private double? _lambdaSmooth;
    private double? _muSmooth;

    private const double Alpha = 0.3;

    private double Smooth(ref double? prev, double current)
    {
        if (prev == null)
            prev = current;
        else
            prev = Alpha * current + (1 - Alpha) * prev.Value;

        return prev.Value;
    }

    // ======================
    // ERROR SMOOTHING
    // ======================
    private double? _errLSmooth;
    private double? _errWSmooth;
    private double? _errWqSmooth;

    private const double ErrAlpha = 0.2;

    private double SmoothError(ref double? prev, double current)
    {
        if (prev == null)
            prev = current;
        else
            prev = ErrAlpha * current + (1 - ErrAlpha) * prev.Value;

        return prev.Value;
    }

    public double SmoothErrL(double err) => SmoothError(ref _errLSmooth, err);
    public double SmoothErrW(double err) => SmoothError(ref _errWSmooth, err);
    public double SmoothErrWq(double err) => SmoothError(ref _errWqSmooth, err);

    // ======================
    // INPUT PARAMS
    // ======================
    private double? _lambdaInput;
    private double? _muInput;
    private ExecutionMode _modeInput;

    public void SetInputParams(double? lambda, double? mu, ExecutionMode mode)
    {
        if (lambda.HasValue)
            _lambdaInput = lambda;

        if (mu.HasValue)
            _muInput = mu;

        _modeInput = mode;
    }

    public ExecutionMode ModeInput() => _modeInput;
    public double? LambdaInput() => _lambdaInput;
    public double? MuInput() => _muInput;

    // ======================
    // EVENTS
    // ======================
    public void MessageReceived()
    {
        _arrivals.Enqueue(DateTime.UtcNow);
        Cleanup(_arrivals);
    }

    public void AddServiceTime(double s)
    {
        _serviceTimes.Enqueue((DateTime.UtcNow, s));
        Cleanup(_serviceTimes);
    }

    public void AddTimes(double total, double wait)
    {
        var now = DateTime.UtcNow;

        _totalTimes.Enqueue((now, total));
        _waitTimes.Enqueue((now, wait));

        Cleanup(_totalTimes);
        Cleanup(_waitTimes);
    }

    public void AddLSample(double l)
    {
        _lSamples.Enqueue((DateTime.UtcNow, l));
        Cleanup(_lSamples);
    }

    public void StartProcessing() => Interlocked.Increment(ref _inProcessing);
    public void EndProcessing() => Interlocked.Decrement(ref _inProcessing);

    public int InProcessing() => _inProcessing;

    // ======================
    // METRICS
    // ======================

    public double Lambda()
    {
        Cleanup(_arrivals);

        if (_arrivals.IsEmpty)
        {
            _lambdaSmooth = 0;
            return 0;
        }

        var raw = _arrivals.Count / _window.TotalSeconds;
        return Smooth(ref _lambdaSmooth, raw);
    }

    public double Mu()
    {
        Cleanup(_serviceTimes);

        if (_serviceTimes.IsEmpty)
            return 0;

        var avg = _serviceTimes.Average(x => x.value);
        var raw = avg > 0 ? 1.0 / avg : 0;

        return Smooth(ref _muSmooth, raw);
    }

    public double AvgTotalTime()
    {
        Cleanup(_totalTimes);
        return _totalTimes.Any() ? _totalTimes.Average(x => x.value) : 0;
    }

    public double AvgWaitTime()
    {
        Cleanup(_waitTimes);
        return _waitTimes.Any() ? _waitTimes.Average(x => x.value) : 0;
    }

    public double AvgL()
    {
        Cleanup(_lSamples);
        return _lSamples.Any() ? _lSamples.Average(x => x.value) : 0;
    }

    // ======================
    // CLEANUP
    // ======================
    private void Cleanup(ConcurrentQueue<DateTime> queue)
    {
        var now = DateTime.UtcNow;

        while (queue.TryPeek(out var item))
        {
            if (now - item > _window)
                queue.TryDequeue(out _);
            else
                break;
        }
    }

    private void Cleanup(ConcurrentQueue<(DateTime time, double value)> queue)
    {
        var now = DateTime.UtcNow;

        while (queue.TryPeek(out var item))
        {
            if (now - item.time > _window)
                queue.TryDequeue(out _);
            else
                break;
        }
    }
}