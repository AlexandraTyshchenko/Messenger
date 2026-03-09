using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Messenger.Infrastructure.Health;

public class RedisHealthPublisher : IHealthCheckPublisher
{
    private readonly RedisStateService _state;

    public RedisHealthPublisher(RedisStateService state)
    {
        _state = state;
    }

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        var redis = report.Entries["redis"];

        var isAvailable = redis.Status == HealthStatus.Healthy;
        _state.IsAvailable = isAvailable;

        return Task.CompletedTask;
    }
}