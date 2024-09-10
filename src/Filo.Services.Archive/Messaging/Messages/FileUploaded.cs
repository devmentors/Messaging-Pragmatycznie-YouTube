using Filo.Shared.Infrastructure.Messaging;

namespace Filo.Services.Archive.Messaging.Messages;

public record FileUploaded(string AbsolutePath, string Name) : IMessage;
