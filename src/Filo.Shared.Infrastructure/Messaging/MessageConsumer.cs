using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Filo.Shared.Infrastructure.Messaging;

internal sealed class MessageConsumer(IConnection connection) : IMessageConsumer
{
    public async Task OnMessageReceived<TMessage>(string queue, Action<TMessage> handle, CancellationToken cancellationToken)
        where TMessage : class, IMessage
    {
        var taskCompletionSource = new TaskCompletionSource();
        using var channel = connection.CreateModel();
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var messageBytes = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(messageBytes);
            var message = JsonSerializer.Deserialize<TMessage>(json);
            handle(message);
        };
        
        channel.BasicConsume(queue: queue,
            autoAck: true,
            consumer: consumer);

        cancellationToken.Register(() => taskCompletionSource.SetCanceled(cancellationToken));
        await taskCompletionSource.Task;
    }
}