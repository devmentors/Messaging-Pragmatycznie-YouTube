namespace Filo.Shared.Infrastructure.Messaging;

public interface IMessagePublisher
{
    void Publish<TMessage>(TMessage message, string exchange) where TMessage : class, IMessage;
    void PublishRespectingPartitionKey<TMessage>(TMessage message, string exchange) where TMessage : class, IMessage;
}