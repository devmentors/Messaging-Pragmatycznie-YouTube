using RabbitMQ.Client;

namespace Filo.Shared.Infrastructure.Messaging;

public interface IMessageConsumer
{
    Task OnMessagesReceived(string queue, Func<string, IBasicProperties, Task> handle,
        CancellationToken cancellationToken);
}