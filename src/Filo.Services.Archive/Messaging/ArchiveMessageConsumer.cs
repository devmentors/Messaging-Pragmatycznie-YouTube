using Filo.Services.Archive.Messaging.Messages;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;

namespace Filo.Services.Archive.Messaging;

public class ArchiveMessageConsumer(IServiceProvider serviceProvider, ILogger<ArchiveMessageConsumer> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
        var inbox = scope.ServiceProvider.GetRequiredService<IMessageInbox>();
        
        await consumer.OnMessageReceived<FileUploaded>("archive-queue",message =>
        {
            logger.LogInformation($"[ARCHIVE SERVICE] Received message of type: " +
                                  $"{message.GetType().Name}. Path: {message.AbsolutePath}, Filename: {message.Name}");
            
            inbox.Add(message);
            
        }, cancellationToken);
    }
}