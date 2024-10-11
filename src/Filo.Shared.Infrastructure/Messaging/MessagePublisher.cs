using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Filo.Shared.Infrastructure.Messaging;

internal sealed class MessagePublisher(IConnection connection) : IMessagePublisher
{
    public void Publish<TMessage>(TMessage message, string exchange) where TMessage : class, IMessage
    {
        var json = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(json);
        
        using var channel = connection.CreateModel();
        var basicProperties = channel.CreateBasicProperties();
        basicProperties.Type = message.GetType().Name;
        
        channel.BasicPublish(exchange: exchange,
            routingKey: string.Empty,
            basicProperties: basicProperties,
            body: messageBytes);
    }
}