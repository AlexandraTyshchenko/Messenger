using Messenger.Infrastructure.Cache;
using Messenger.Infrastructure.CachedRepositories;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Health;
using Messenger.Infrastructure.Interfaces;
using Messenger.Infrastructure.KeyBuilder;
using Messenger.Infrastructure.Repositories;
using Messenger.Infrastructure.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Messenger.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConnectionRepository, ConnectionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
         // 
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ICacheKeyBuilderFactory, CacheKeyBuilderFactory>();

        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.Decorate<IParticipantRepository, CachedParticipantRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.Decorate<IUserRepository, CachedUserRepository>();

        services.AddHealthChecks()
            .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis");

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(2);  
            options.Period = TimeSpan.FromSeconds(30); 
        });

        services.AddSingleton<RedisStateService>();
        return services;
    }
}
