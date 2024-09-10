using Filo.FilesService.Messages;
using Filo.Shared.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRabbitMq(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Files Service");
app.MapGet("/upload", (IMessagePublisher messagePublisher) =>
{
    var message = new FileUploaded("PATH", $@"File_{++FileNumber.Value}.pdf");
    messagePublisher.Publish(message, exchange: "files-exchange");
    return Results.Accepted();
});

app.Run();


static class FileNumber
{
    public static int Value { get; set; }
}