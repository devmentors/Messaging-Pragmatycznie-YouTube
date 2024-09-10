using Filo.Shared.Infrastructure.Messaging;

namespace Filo.FilesService.Messages;

public record FileUploaded(string AbsolutePath, string Name) : IMessage;
