using Filo.Services.Security.Messaging;
using Filo.Services.Security.Messaging.Messages;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHostedService<SecurityMessageConsumer>()
    .AddRabbitMq(builder.Configuration)
    .AddMessageInbox();

var app = builder.Build();

app.MapGet("/", () => "Security Service");
app.MapGet("/files", (IMessageInbox storage) => storage.GetAll());

app.MapPost("/files",
    (ILogger<Program> logger, IMessageInbox inbox, FileUploaded message) =>
        SecurityMessageConsumer.ConsumeLogic(logger, inbox, message));

app.Run();