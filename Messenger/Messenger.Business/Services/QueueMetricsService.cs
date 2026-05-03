using Messenger.Business.Options;
using Microsoft.Extensions.Options;

namespace Messenger.Business.Services;

public class QueueMetricsService
{
    private readonly WorkerSettings _settings;

    public QueueMetricsService(IOptions<WorkerSettings> options)
    {
        _settings = options.Value;
        _startTime = DateTime.UtcNow;
        _lastUpdate = _startTime;
    }

    // ======================
    // COUNTERS
    // ======================

    private long _arrivals = 0;
    private long _completed = 0;

    private double _sumW = 0;
    private double _sumWq = 0;
    private double _sumService = 0;

    private readonly object _lock = new();

    // ======================
    // L / Lq (time-weighted)
    // ======================

    private double _areaL = 0;
    private double _areaLq = 0;

    private int _currentL = 0;
    private int _currentLq = 0;

    private DateTime _lastUpdate;
    private readonly DateTime _startTime;

    private int _inProcessing = 0;

    // ======================
    // EVENTS
    // ======================

    public void MessageReceived(int queueLength)
    {
        Interlocked.Increment(ref _arrivals);
        UpdateL(queueLength);
    }

    public void StartProcessing(int queueLength)
    {
        Interlocked.Increment(ref _inProcessing);
        UpdateL(queueLength);
    }

    public void EndProcessing(int queueLength)
    {
        Interlocked.Decrement(ref _inProcessing);
        UpdateL(queueLength);
    }

    public void AddTimes(double total, double wait, double service)
    {
        lock (_lock)
        {
            _completed++;
            _sumW += total;
            _sumWq += wait;
            _sumService += service;
        }
    }

    // ======================
    // CORE: time-weighted L
    // ======================

    private void UpdateL(int queueLength)
    {
        var now = DateTime.UtcNow;
        var dt = (now - _lastUpdate).TotalSeconds;

        _areaL += _currentL * dt;
        _areaLq += _currentLq * dt;

        _currentL = queueLength + _inProcessing;
        _currentLq = queueLength;

        _lastUpdate = now;
    }

    // ======================
    // METRICS
    // ======================

    public double W()
    {
        lock (_lock)
            return _completed > 0 ? _sumW / _completed : 0;
    }

    public double Wq()
    {
        lock (_lock)
            return _completed > 0 ? _sumWq / _completed : 0;
    }

    public double Mu()
    {
        lock (_lock)
        {
            if (_completed == 0) return 0;
            var avgService = _sumService / _completed;
            return avgService > 0 ? 1.0 / avgService : 0;
        }
    }

    public double Lambda()
    {
        var totalTime = (DateTime.UtcNow - _startTime).TotalSeconds;
        return totalTime > 0 ? _arrivals / totalTime : 0;
    }

    public double L()
    {
        var totalTime = (DateTime.UtcNow - _startTime).TotalSeconds;
        return totalTime > 0 ? _areaL / totalTime : 0;
    }

    public double Lq()
    {
        var totalTime = (DateTime.UtcNow - _startTime).TotalSeconds;
        return totalTime > 0 ? _areaLq / totalTime : 0;
    }

    public double Rho()
    {
        var lambda = Lambda();
        var mu = Mu();

        if (mu == 0) return 0;

        return lambda / (_settings.WorkerCount * mu);
    }

    public int InProcessing() => _inProcessing;

    public long Completed => _completed;
}