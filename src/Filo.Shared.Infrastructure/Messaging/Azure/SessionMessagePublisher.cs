using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Filo.Shared.Infrastructure.Messaging.Azure;

public class SessionMessagePublisher(AzureServiceBusConnectionString connectionString)
{
    public async Task PublishAsync<TMessage>(TMessage message, string topic, CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        await using ServiceBusClient client = new ServiceBusClient(connectionString.Value);
        ServiceBusSender sender = client.CreateSender(topic);

        var json = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(json);
        
        ServiceBusMessage busMessage = new(messageBytes)
        {
            SessionId = message.PartitionKey,
            ApplicationProperties = { { "Type", message.GetType().Name } }
        };

        await sender.SendMessageAsync(busMessage, cancellationToken);
    }
}