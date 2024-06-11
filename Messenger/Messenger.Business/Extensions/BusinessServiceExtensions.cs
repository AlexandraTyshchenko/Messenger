using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Business.Extensions
{
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetAssembly(typeof(GetConversationByUserId.QueryHandler)));

            return services;
        }
    }
}
