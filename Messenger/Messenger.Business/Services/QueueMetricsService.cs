namespace Messenger.Business.Services;

public class QueueMetricsService
{
    private long _receivedMessages = 0;
    private long _processedMessages = 0;

    private readonly DateTime _startTime = DateTime.UtcNow;

    private const int Servers = 1;

    public void MessageReceived()
    {
        Interlocked.Increment(ref _receivedMessages);
    }

    public void MessageProcessed()
    {
        Interlocked.Increment(ref _processedMessages);
    }

    public double Lambda()
    {
        var seconds = (DateTime.UtcNow - _startTime).TotalSeconds;
        if (seconds == 0) return 0;

        return _receivedMessages / seconds;
    }

    public double Mu()
    {
        var seconds = (DateTime.UtcNow - _startTime).TotalSeconds;
        if (seconds == 0) return 0;

        return _processedMessages / seconds;
    }

    public double Rho()
    {
        var lambda = Lambda();
        var mu = Mu();

        if (mu == 0) return 0;

        return lambda / (Servers * mu);
    }

    public bool IsOverloaded()
    {
        return Rho() > 1;
    }
}