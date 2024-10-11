using Filo.FilesService.Messages;
using Filo.Shared.Infrastructure.Http;
using Filo.Shared.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRabbitMq(builder.Configuration);
builder.Services.AddHttp();

var app = builder.Build();

app.MapGet("/", () => "Files Service");
app.MapPost("/upload", (IMessagePublisher messagePublisher) =>
{
    var message = new FileUploaded("PATH", $@"File_{++FileNumber.Value}.pdf");
    messagePublisher.Publish(message, exchange: "files-exchange");
    return Results.Accepted();
});

app.MapPost("/rename", (IMessagePublisher messagePublisher) =>
{
    var message = new FileRenamed($"File_{FileNumber.Value}.pdf", $"File_New_{FileNumber.Value}.pdf");
    messagePublisher.Publish(message, exchange: "files-exchange");
    return Results.Accepted();
});


app.Run();


static class FileNumber
{
    public static int Value { get; set; }
}