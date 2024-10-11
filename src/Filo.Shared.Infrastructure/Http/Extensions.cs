using Microsoft.Extensions.DependencyInjection;

namespace Filo.Shared.Infrastructure.Http;

public static class Extensions
{
    public static IServiceCollection AddHttp(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IMulticastHttpClient, MulticastHttpClient>();

        return services;
    }
}