using Filo.Shared.Infrastructure.Messaging;

namespace Filo.FilesService.Messages;

public record FileUploaded(int FileNumber, string AbsolutePath, string Name) : IMessage
{
    public string PartitionKey => FileNumber.ToString();
}
