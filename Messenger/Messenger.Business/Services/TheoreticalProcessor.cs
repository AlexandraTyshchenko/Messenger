using Messenger.Business.Dtos;
using Messenger.Business.EventBus;
using Messenger.Business.Interfaces;

namespace Messenger.Business.Services;

public class TheoreticalProcessor : IMessageProcessor
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());

    public async Task ProcessAsync(EventMessage eventMessage, CancellationToken token)
    {
        var mu = eventMessage.Mu ?? 2.0;

        var delayMs = GetExponentialDelayMs(mu);
        await Task.Delay(delayMs, token);
    }

    private int GetExponentialDelayMs(double mu)
    {
        if (mu <= 0)
            throw new ArgumentException("Mu must be > 0");

        var u = _random.Value!.NextDouble();

        if (u == 0)
            u = double.Epsilon;

        var delaySeconds = -Math.Log(u) / mu;

        return (int)(delaySeconds * 1000);
    }
}