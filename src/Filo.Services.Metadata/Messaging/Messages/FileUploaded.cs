using Filo.Shared.Infrastructure.Messaging;

namespace Filo.Services.Metadata.Messaging.Messages;

public record FileUploaded(string AbsolutePath, string Name) : IMessage;
