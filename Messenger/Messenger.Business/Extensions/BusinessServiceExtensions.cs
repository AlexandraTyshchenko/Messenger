using AutoMapper;
using MediatR;
using Messenger.Business.Profiles;
using Messenger.Business.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Extensions
{
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(GetConversationByUserIdQueryHandler).Assembly);

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
