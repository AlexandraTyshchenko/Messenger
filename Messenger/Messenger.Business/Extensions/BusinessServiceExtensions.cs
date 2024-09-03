using AutoMapper;
using FluentValidation;
using MediatR;
using Messanger.Image.Client.Extensions;
using Messenger.Business.Commands;
using Messenger.Business.Interfaces;
using Messenger.Business.Profiles;
using Messenger.Business.Queries;
using Messenger.Business.Services;
using Messenger.Business.ValidationPipelines;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Extensions;

public static class BusinessServiceExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
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

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUrlHelperService, UrlHelperService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IHubService, HubService>();
        services.AddScoped<IAuthHeaderService, AuthHeaderService>();

        return services;
    }
}
