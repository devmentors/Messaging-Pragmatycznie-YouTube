namespace Filo.Shared.Infrastructure.Messaging;

// IMessage
public interface IMessage
{
    string PartitionKey { get; }
}