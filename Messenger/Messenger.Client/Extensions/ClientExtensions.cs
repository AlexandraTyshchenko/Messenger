using Messanger.Image.Client.Options;
using Messanger.Image.Client.Services;
using Messenger.Client.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Messanger.Image.Client.Extensions;

public static class ClientExtensions
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        services.AddScoped<IImageClient, ImageClient>();
        services.Configure<ImageServiceSettings>(builder.Configuration.GetSection("ImageServiceSettings"));
        return services;
    }
}
