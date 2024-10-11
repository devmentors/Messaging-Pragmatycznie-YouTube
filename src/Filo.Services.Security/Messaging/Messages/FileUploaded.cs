using Filo.Shared.Infrastructure.Messaging;

namespace Filo.Services.Security.Messaging.Messages;

public record FileUploaded(int FileNumber, string AbsolutePath, string Name) : IMessage
{
    public string PartitionKey { get; }
}
