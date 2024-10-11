using RabbitMQ.Client;

namespace Filo.Shared.Infrastructure.Messaging;

public interface IMessageConsumer
{
    Task OnMessageReceived<TMessage>(string queue, Func<TMessage, IBasicProperties, Task> handle, CancellationToken cancellationToken) 
        where TMessage : class, IMessage;

    Task OnMessagesReceived(string queue, Func<string, IBasicProperties, Task> handle,
        CancellationToken cancellationToken);
}