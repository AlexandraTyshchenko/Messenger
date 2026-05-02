using Messenger.Business.Dtos;
using Messenger.Business.EventBus;
using Messenger.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Services;

public class TheoreticalProcessor : IMessageProcessor
{
    private static readonly Random _random = new();

    public TheoreticalProcessor() {}

    public async Task ProcessAsync(EventMessage eventMessage, CancellationToken token)
    {
        var mu = eventMessage.Mu.HasValue ? eventMessage.Mu.Value : 2.0;
        
        var delayMs = GetExponentialDelayMs(mu);
        await Task.Delay(delayMs, token);
    }


    private int GetExponentialDelayMs(double mu)
    {
        var u = _random.NextDouble();
        var delaySeconds = -Math.Log(u) / mu;
        return (int)(delaySeconds * 1000);
    }
}
