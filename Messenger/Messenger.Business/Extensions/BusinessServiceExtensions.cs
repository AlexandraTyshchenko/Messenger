using AutoMapper;
using MediatR;
using Messenger.Business.Options;
using Messenger.Business.Profiles;
using Messenger.Business.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Extensions
{
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(GetConversationsByUserIdQueryHandler).Assembly);

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}
