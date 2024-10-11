using Filo.Shared.Infrastructure.Messaging;

namespace Filo.Services.Archive.Messaging.Messages;

public record FileRenamed(string OldName, string NewName) : IMessage;