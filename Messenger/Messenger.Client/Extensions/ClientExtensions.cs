using Messanger.Image.Client.Services;
using Messenger.Client.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Messanger.Image.Client.Extensions;

public static class ClientExtensions
{
    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddScoped<IImageClient, ImageClient>();

        return services;
    }
}
