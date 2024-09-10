using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Filo.Shared.Infrastructure.Messaging;

public static class Extensions
{
    private const string ConfigSection = "RabbitMq";
    
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var hostName = configuration.GetValue<string>("RabbitMq:HostName");
        var factory = new ConnectionFactory { HostName = hostName };
        var connection = factory.CreateConnection();

        services.AddSingleton(connection);
        services.AddSingleton<IMessagePublisher, MessagePublisher>();
        services.AddSingleton<IMessageConsumer, MessageConsumer>();
        
        return services;
    }
}