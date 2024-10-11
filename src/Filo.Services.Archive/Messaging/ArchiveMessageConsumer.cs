using System.Text.Json;
using Filo.Services.Archive.Messaging.Messages;
using Filo.Services.Archive.Storage;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Tools.RabbitMqTopology;

namespace Filo.Services.Archive.Messaging;

public class ArchiveMessageConsumer
    : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ArchiveMessageConsumer> _logger;
    private readonly int PartitionNum; 
    
    public ArchiveMessageConsumer(IServiceProvider serviceProvider, ILogger<ArchiveMessageConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        //Usage: dotnet run -- PartitionNum=<value> 
        var partition = configuration.GetValue<int>("PartitionNum");
        
        
        if (partition is 0)
        {
            _logger.LogWarning($"[PARTITION ASSIGNMENT] Fallback to {nameof(PartitionNum)}: {PartitionNum}... This might lead to unexpected competing consumers behavior kicking in!");
            _logger.LogWarning($"[PARTITION ASSIGNMENT] You can safely ignore this if you're testing topic-per-stream, not CHE example.");
            partition = 1;
        }
        
        if (partition <= 0 || partition > ArchivePartitions.FilesExchange.PartitionCount)
        {
            throw new ArgumentOutOfRangeException(nameof(partition),
                $"Partition should be in range of 1-{ArchivePartitions.FilesExchange.PartitionCount}");
        }

        PartitionNum = partition;
        
        _logger.LogInformation($"[PARTITION ASSIGNMENT] {nameof(PartitionNum)}: {PartitionNum}");
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var scope = _serviceProvider.CreateScope();
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

        // await consumer.OnMessagesReceived(Queues.Archive,async (messageJson, properties) =>
        // {
        //     if (properties.Type is nameof(FileUploaded))
        //     {
        //         var message = JsonSerializer.Deserialize<FileUploaded>(messageJson);
        //         await ProcessFileUploadedAsync(message, storage, cancellationToken);
        //         return;
        //     }
        //     if (properties.Type is nameof(FileRenamed))
        //     {
        //         var message = JsonSerializer.Deserialize<FileRenamed>(messageJson);
        //         await ProcessFileRenamedAsync(message, storage, cancellationToken);
        //     }
        //
        // }, cancellationToken);
                
#endregion           

#region partition-queue-che

await consumer.OnMessagesReceived(ArchivePartitions.FilesExchange.GetQueueNameForPartitionNum(PartitionNum),async (messageJson, properties) =>
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
        _logger.LogInformation($"[ARCHIVE SERVICE] Received message of type: " +
                              $"{message.GetType().Name}. Path: {message.AbsolutePath}, Filename: {message.Name}");

        await Task.Delay(10_000, cancellationToken); // DOWNLOADING FILE HERE
        storage.AddFile(message.Name, message.AbsolutePath);
        
        _logger.LogInformation($"[ARCHIVE SERVICE] Processed message of type: " +
                              $"{message.GetType().Name}. Path: {message.AbsolutePath}, Filename: {message.Name}");
    }
    
    private async Task ProcessFileRenamedAsync(FileRenamed message, FilesStorage storage, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[ARCHIVE SERVICE] Received message of type: " +
                              $"{message.GetType().Name}. Old name: {message.OldName}, New name: {message.NewName}");

        storage.RenameFile(message.OldName, message.NewName);
    }
}