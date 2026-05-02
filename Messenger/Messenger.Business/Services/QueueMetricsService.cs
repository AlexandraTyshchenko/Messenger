namespace Messenger.Business.Services;

using Messenger.Business.Options;
using Microsoft.Extensions.Options;

public class QueueMetricsService
{
    private long _receivedMessages = 0;
    private long _processedMessages = 0;

    private const int WindowSeconds = 15;

    private readonly Queue<DateTime> _arrivalTimes = new();
    private readonly object _arrivalLock = new();

    private readonly Queue<(DateTime time, double serviceTime)> _serviceTimes = new();
    private readonly object _serviceLock = new();

    private readonly WorkerSettings _settings;

    private int _inProcessing = 0;

    private double _totalServiceTime = 0;
    private int _totalProcessed = 0;
    private double _lambdaEma = 0;
    private const double Alpha = 0.15;
    public QueueMetricsService(IOptions<WorkerSettings> options)
    {
        _settings = options.Value;
    }

    public void MessageReceived()
    {
        var now = DateTime.UtcNow;

        lock (_arrivalLock)
        {
            _arrivalTimes.Enqueue(now);

            while (_arrivalTimes.Count > 0 &&
                   (now - _arrivalTimes.Peek()).TotalSeconds > WindowSeconds)
            {
                _arrivalTimes.Dequeue();
            }
        }

        Interlocked.Increment(ref _receivedMessages);
    }

    public void MessageProcessed()
    {
        Interlocked.Increment(ref _processedMessages);
    }

    public double Lambda()
    {
        double raw;

        lock (_arrivalLock)
        {
            raw = _arrivalTimes.Count / (double)WindowSeconds;
        }

        if (_lambdaEma == 0)
            _lambdaEma = raw;
        else
            _lambdaEma = Alpha * raw + (1 - Alpha) * _lambdaEma;

        return _lambdaEma;
    }

    public void AddServiceTime(double seconds)
    {
        var now = DateTime.UtcNow;

        lock (_serviceLock)
        {
            _serviceTimes.Enqueue((now, seconds));

            while (_serviceTimes.Count > 0 &&
                   (now - _serviceTimes.Peek().time).TotalSeconds > WindowSeconds)
            {
                _serviceTimes.Dequeue();
            }

            _totalServiceTime += seconds;
            _totalProcessed++;
        }
    }

    public double MuReal()
    {
        lock (_serviceLock)
        {
            if (_totalProcessed < 5)
                return _settings.Mu;

            return _totalProcessed / _totalServiceTime;
        }
    }

    public void StartProcessing()
    {
        Interlocked.Increment(ref _inProcessing);
    }

    public void EndProcessing()
    {
        Interlocked.Decrement(ref _inProcessing);
    }

    public int InProcessing()
    {
        return _inProcessing;
    }

    public double Rho()
    {
        var lambda = Lambda();
        var mu = MuReal();

        return lambda / (_settings.WorkerCount * mu);
    }

    public bool IsOverloaded()
    {
        return Rho() > 1;
    }
}