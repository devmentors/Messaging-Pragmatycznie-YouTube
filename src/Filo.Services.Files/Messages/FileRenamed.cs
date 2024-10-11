using Filo.Shared.Infrastructure.Messaging;

namespace Filo.FilesService.Messages;

public record FileRenamed(int FileNumber, string OldName, string NewName) : IMessage
{
    public string PartitionKey => FileNumber.ToString();
}