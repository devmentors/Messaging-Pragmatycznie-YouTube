using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Filo.Shared.Infrastructure.Messaging;

internal sealed class MessageConsumer(IConnection connection, ILogger<MessageConsumer> logger) : IMessageConsumer
{
    public async Task OnMessagesReceived(string queue, Func<string, IBasicProperties, Task> handle, CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource();
        using var channel = connection.CreateModel();
        
        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var messageBytes = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(messageBytes);
            var properties = ea.BasicProperties;
            try
            {
                await handle(json, properties);

                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                logger.LogError("Consume failed", ex);
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };
        
        channel.BasicConsume(queue: queue,
            autoAck: false,
            consumer: consumer);

        cancellationToken.Register(() => taskCompletionSource.SetCanceled(cancellationToken));
        await taskCompletionSource.Task;
    }
}