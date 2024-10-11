using Filo.Services.Metadata.Messaging.Messages;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;

namespace Filo.Services.Metadata.Messaging;

public class MetadataMessageConsumer(IServiceProvider serviceProvider, ILogger<MetadataMessageConsumer> logger) 
    : BackgroundService
{
    public static readonly Action<ILogger, IMessageInbox, FileUploaded> ConsumeLogic =
        (messageLogger, messageInbox, message) =>
        {
            messageLogger.LogInformation($"[METADATA SERVICE] Received message of type: " +
                                         $"{message.GetType().Name}. Path: {message.AbsolutePath}, Filename: {message.Name}");
            
            messageInbox.Add(message);
        };
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
        var inbox = scope.ServiceProvider.GetRequiredService<IMessageInbox>();
        
        await consumer.OnMessageReceived<FileUploaded>("metadata-queue",async (message, _) =>
        {
            ConsumeLogic(logger, inbox, message);

        }, cancellationToken);
    }
}