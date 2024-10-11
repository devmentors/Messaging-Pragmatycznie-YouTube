using Filo.Services.Security.Messaging.Messages;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;

namespace Filo.Services.Security.Messaging;

public class SecurityMessageConsumer(IServiceProvider serviceProvider, ILogger<SecurityMessageConsumer> logger) 
    : BackgroundService
{
    public static readonly Action<ILogger, IMessageInbox, FileUploaded> ConsumeLogic =
        (messageLogger, messageInbox, message) =>
        {
            messageLogger.LogInformation($"[SECURITY SERVICE] Received message of type: " +
                                         $"{message.GetType().Name}. Path: {message.AbsolutePath}, Filename: {message.Name}");
            
            messageInbox.Add(message);
        };
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
        var inbox = scope.ServiceProvider.GetRequiredService<IMessageInbox>();
        
        // await consumer.OnMessageReceived<FileUploaded>("security-queue",message =>
        // {
        //     ConsumeLogic(logger, inbox, message);
        //     
        // }, cancellationToken);
    }
}