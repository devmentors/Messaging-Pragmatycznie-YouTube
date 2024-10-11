using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Filo.Shared.Infrastructure.Messaging;

internal sealed class MessageConsumer(IConnection connection) : IMessageConsumer
{
    public async Task OnMessageReceived<TMessage>(string queue, Func<TMessage, IBasicProperties, Task> handle, CancellationToken cancellationToken)
        where TMessage : class, IMessage
    {
        var taskCompletionSource = new TaskCompletionSource();
        using var channel = connection.CreateModel();
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var messageBytes = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(messageBytes);
            var message = JsonSerializer.Deserialize<TMessage>(json);
            var properties = ea.BasicProperties;
            await handle(message, properties);
        };
        
        channel.BasicConsume(queue: queue,
            autoAck: true,
            consumer: consumer);

        cancellationToken.Register(() => taskCompletionSource.SetCanceled(cancellationToken));
        await taskCompletionSource.Task;
    }
    
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
            await handle(json, properties);
            
            channel.BasicAck(ea.DeliveryTag, false);
        };
        
        channel.BasicConsume(queue: queue,
            autoAck: false,
            consumer: consumer);

        cancellationToken.Register(() => taskCompletionSource.SetCanceled(cancellationToken));
        await taskCompletionSource.Task;
    }
}