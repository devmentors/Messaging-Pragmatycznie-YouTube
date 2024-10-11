using Filo.Services.Archive.Messaging;
using Filo.Services.Archive.Storage;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHostedService<ArchiveMessageConsumer>()
    .AddSingleton<FilesStorage>()
    .AddRabbitMq(builder.Configuration)
    .AddMessageInbox()
    .AddAzureServiceBus(builder.Configuration)
    .AddHostedService<ArchiveSessionMessageConsumer>();

var app = builder.Build();

app.MapGet("/", () => "Archive Service");
app.MapGet("/files", (IMessageInbox storage) => storage.GetAll());

// app.MapPost("/files",
//     (ILogger<Program> logger, IMessageInbox inbox, FileUploaded message) =>
//         ArchiveMessageConsumer.ConsumeLogic(logger, inbox, message));

app.Run();

