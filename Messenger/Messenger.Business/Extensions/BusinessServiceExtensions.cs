using AutoMapper;
using MediatR;
using Messenger.Business.Profiles;
using Messenger.Business.Queries;
using Messenger.Business.ValidationPipelines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
        return services;
    }
}
