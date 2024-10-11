using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Filo.Services.Archive.Messaging.Messages;
using Filo.Services.Archive.Storage;
using Filo.Shared.Infrastructure.Messaging.Azure;
using Filo.Tools.RabbitMqTopology;

namespace Filo.Services.Archive.Messaging;

public class ArchiveSessionMessageConsumer(
    AzureServiceBusConnectionString connectionString, 
    ILogger<ArchiveSessionMessageConsumer> logger,
    IServiceProvider serviceProvider) 
    : BackgroundService
{
    private ServiceBusSessionProcessor _processor;
    private ServiceBusClient _client;
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _client = new ServiceBusClient(connectionString.Value);
        
        var options = new ServiceBusSessionProcessorOptions
        {
            MaxConcurrentSessions = 4, //Ile sesji na raz obslugujemy?
            MaxConcurrentCallsPerSession = 1 //Ile wiadomosci na raz w ramach sesji chcemy przetwarzac?
        };

        _processor = _client.CreateSessionProcessor(
            Topics.Files.Default,
            Subscriptions.Files.Archive, 
            options);

        _processor.ProcessMessageAsync += OnMessageAsync(cancellationToken);
        _processor.ProcessErrorAsync += _ => Task.CompletedTask;

        await base.StartAsync(cancellationToken);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private Func<ProcessSessionMessageEventArgs, Task> OnMessageAsync(CancellationToken cancellationToken)
    {
        return async args =>
        {
            using var iocScope = serviceProvider.CreateScope();
            var storage = iocScope.ServiceProvider.GetRequiredService<FilesStorage>();
            
            var payload = args.Message;
            var properties = payload.ApplicationProperties;

            if (properties.ContainsKey("Type") is false)
            {
                logger.LogError("'Type' property not found in message! Skipping...");
                return;
            }
            
            if (properties["Type"] is nameof(FileUploaded))
            {
                var message = JsonSerializer.Deserialize<FileUploaded>(payload.Body);
                await ProcessFileUploadedAsync(message, storage, cancellationToken);
                return;
            }
            if (properties["Type"] is nameof(FileRenamed))
            {
                var message = JsonSerializer.Deserialize<FileRenamed>(payload.Body);
                await ProcessFileRenamedAsync(message, storage, cancellationToken);
            }
        };
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisposeAsync();
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
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

    private async Task ProcessFileRenamedAsync(FileRenamed message, FilesStorage storage,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"[ARCHIVE SERVICE] Received message of type: " +
                               $"{message.GetType().Name}. Old name: {message.OldName}, New name: {message.NewName}");

        storage.RenameFile(message.OldName, message.NewName);

    }
}