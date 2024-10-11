using Filo.FilesService.Messages;
using Filo.Shared.Infrastructure.Http;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Messaging.Azure;
using Filo.Tools.RabbitMqTopology;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddRabbitMq(builder.Configuration)
    .AddAzureServiceBus(builder.Configuration);
builder.Services.AddHttp();

var app = builder.Build();

app.MapGet("/", () => "Files Service");
app.MapGet("/upload", (IMessagePublisher messagePublisher) =>
{
    var fileNumber = ++FileNumber.Value;
    var fileUploaded = new FileUploaded(fileNumber, "PATH", $"File_{fileNumber}.pdf");
    var fileRenamed = new FileRenamed(fileNumber, $"File_{fileNumber}.pdf", $"Plik_{fileNumber}.pdf");
    
    // messagePublisher.Publish(fileUploaded, exchange: Exchanges.FilesExchange.Default);
    // messagePublisher.Publish(fileRenamed, exchange: Exchanges.FilesExchange.Default);
    
    // messagePublisher.PublishRespectingPartitionKey(fileUploaded, Exchanges.FilesExchange.ConsistentHash);
    // messagePublisher.PublishRespectingPartitionKey(fileRenamed, Exchanges.FilesExchange.ConsistentHash);
    return Results.Accepted();
});

app.MapGet("/upload-azure", async (SessionMessagePublisher sessionPublisher, CancellationToken cancellationToken) =>
 {
    var fileNumber = ++FileNumber.Value;
    var fileUploaded = new FileUploaded(fileNumber, "PATH", $"File_{fileNumber}.pdf");
    var fileRenamed = new FileRenamed(fileNumber, $"File_{fileNumber}.pdf", $"Plik_{fileNumber}.pdf");

    await sessionPublisher.PublishAsync(fileUploaded, Topics.Files.Default, cancellationToken);
    await sessionPublisher.PublishAsync(fileRenamed, Topics.Files.Default, cancellationToken);
});

app.MapPost("/upload-sync", async ([FromServices] IMulticastHttpClient client, [FromQuery] bool parallelSend = false) =>
{
    var fileNumber = ++FileNumber.Value;
    var message = new FileUploaded(fileNumber, "PATH", $"File_{fileNumber}.pdf");

    Uri[] recipientsUris =
    [
        new("http://localhost:5207/files" /* Metadata */),
        new("http://localhost:5274/files" /* Security */),
        new("http://localhost:5292/files" /* Archive */)
    ];

    if (parallelSend is false)
    {
        await client.PostInSequenceAsync(recipientsUris, message, CancellationToken.None);
    }
    else
    {
        await client.PostInParallelAsync(recipientsUris, message, CancellationToken.None);
    }
});

app.Run();


static class FileNumber
{
    public static int Value { get; set; }
}