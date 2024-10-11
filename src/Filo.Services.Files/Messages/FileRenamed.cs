using Filo.Shared.Infrastructure.Messaging;

namespace Filo.FilesService.Messages;

public record FileRenamed(string OldName, string NewName) : IMessage;