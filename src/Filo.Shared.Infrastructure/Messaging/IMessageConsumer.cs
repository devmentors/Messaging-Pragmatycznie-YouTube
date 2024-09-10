namespace Filo.Shared.Infrastructure.Messaging;

public interface IMessageConsumer
{
    Task OnMessageReceived<TMessage>(string queue, Action<TMessage> handle, CancellationToken cancellationToken) 
        where TMessage : class, IMessage;
}