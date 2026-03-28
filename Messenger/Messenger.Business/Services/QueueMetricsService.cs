namespace Messenger.Business.Services;

using Messenger.Business.Options;
using Microsoft.Extensions.Options;

public class QueueMetricsService
{
    private long _receivedMessages = 0;
    private long _processedMessages = 0;

    private const int WindowSeconds = 1;
    private const int ServiceWindowSize = 500;

    private readonly Queue<DateTime> _arrivalTimes = new();
    private readonly object _arrivalLock = new();

    private readonly Queue<double> _serviceTimes = new();
    private readonly object _serviceLock = new();

    private readonly WorkerSettings _settings;
    private int _inProcessing = 0;

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
        lock (_arrivalLock)
        {
            return _arrivalTimes.Count / (double)WindowSeconds;
        }
    }

    public void AddServiceTime(double seconds)
    {
        lock (_serviceLock)
        {
            _serviceTimes.Enqueue(seconds);

            while (_serviceTimes.Count > ServiceWindowSize)
                _serviceTimes.Dequeue();
        }
    }

    public double MuReal()
    {
        lock (_serviceLock)
        {
            if (_serviceTimes.Count == 0) return 0;

            var avg = _serviceTimes.Average();
            return avg == 0 ? 0 : 1.0 / avg;
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

        if (mu == 0) return 0;

        return lambda / (_settings.WorkerCount * mu);
    }

    public bool IsOverloaded()
    {
        return Rho() > 1;
    }
}
