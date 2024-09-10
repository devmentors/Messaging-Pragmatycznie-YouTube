using Filo.Shared.Infrastructure.Messaging;

namespace Filo.Services.Security.Messaging.Messages;

public record FileUploaded(string AbsolutePath, string Name) : IMessage;
