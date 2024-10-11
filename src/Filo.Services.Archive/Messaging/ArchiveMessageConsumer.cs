using System.Buffers;
using System.Text.Json;
using Filo.Services.Archive.Messaging.Messages;
using Filo.Services.Archive.Storage;
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
        var storage = scope.ServiceProvider.GetRequiredService<FilesStorage>();

#region topic-per-type

        // var fileUploadedConsumeTask =  consumer.OnMessageReceived<FileUploaded>("file-uploaded-queue",async (message,_) =>
        // {
        //     await ProcessFileUploadedAsync(message, storage, cancellationToken);
        //             
        // }, cancellationToken);
        //         
        // var fileRenamedConsumeTask =  consumer.OnMessageReceived<FileRenamed>("file-renamed-queue",async (message,_) =>
        // {
        //     await ProcessFileRenamedAsync(message, storage, cancellationToken);
        //             
        // }, cancellationToken);
        //         
        // await Task.WhenAll(fileUploadedConsumeTask, fileRenamedConsumeTask);
        
#endregion        

#region topic-per-stream

        await consumer.OnMessagesReceived("archive-queue",async (messageJson, properties) =>
        {
            if (properties.Type is nameof(FileUploaded))
            {
                var message = JsonSerializer.Deserialize<FileUploaded>(messageJson);
                await ProcessFileUploadedAsync(message, storage, cancellationToken);
                return;
            }
            if (properties.Type is nameof(FileRenamed))
            {
                var message = JsonSerializer.Deserialize<FileRenamed>(messageJson);
                await ProcessFileRenamedAsync(message, storage, cancellationToken);
            }

        }, cancellationToken);
                
#endregion           
     
    }

    private async Task ProcessFileUploadedAsync(FileUploaded message, FilesStorage storage, CancellationToken cancellationToken)
    {
        logger.LogInformation($"[ARCHIVE SERVICE] Received message of type: " +
                              $"{message.GetType().Name}. Path: {message.AbsolutePath}, Filename: {message.Name}");

        await Task.Delay(10_000, cancellationToken); // DOWNLOADING FILE HERE
        storage.AddFile(message.Name, message.AbsolutePath);
        
        logger.LogInformation($"[ARCHIVE SERVICE] Processed message of type: " +
                              $"{message.GetType().Name}. Path: {message.AbsolutePath}, Filename: {message.Name}");
    }
    
    private async Task ProcessFileRenamedAsync(FileRenamed message, FilesStorage storage, CancellationToken cancellationToken)
    {
        logger.LogInformation($"[ARCHIVE SERVICE] Received message of type: " +
                              $"{message.GetType().Name}. Old name: {message.OldName}, New name: {message.NewName}");

        storage.RenameFile(message.OldName, message.NewName);
    }
}