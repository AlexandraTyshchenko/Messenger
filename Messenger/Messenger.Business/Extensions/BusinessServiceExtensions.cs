using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Commands;
using Messenger.Business.Interfaces;
using Messenger.Business.Options;
using Messenger.Business.Profiles;
using Messenger.Business.Queries;
using Messenger.Business.Queues;
using Messenger.Business.Services;
using Messenger.Business.ValidationPipelines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Extensions;

public static class BusinessServiceExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<GetConversationsByUserIdQueryHandler>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining(typeof(CreateGroupConversationCommandValidator),
            includeInternalTypes: true);

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        services.Configure<EmailConfirmationSettings>(
          configuration.GetSection("EmailConfirmationSettings"));

        services.Configure<SmtpSettings>(
            configuration.GetSection("SmtpSettings"));
        services.Configure<WorkerSettings>(
            configuration.GetSection("WorkerSettings"));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUrlHelperService, UrlHelperService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IHubService, HubService>();

        services.AddSingleton<QueueMetricsService>();
        services.AddSingleton<MessageQueue>();
        services.AddHostedService<MessageWorker>();
        return services;
    }
}
