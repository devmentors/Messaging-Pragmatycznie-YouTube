using Microsoft.Extensions.DependencyInjection;

namespace Filo.Shared.Infrastructure.Storage;

public static class Extensions
{
    public static IServiceCollection AddMessageInbox(this IServiceCollection services)
    {
        services.AddSingleton<IMessageInbox, MessageInbox>();
        return services;
    }
}