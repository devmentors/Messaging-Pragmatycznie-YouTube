using Filo.Services.Archive.Messaging;
using Filo.Services.Archive.Messaging.Messages;
using Filo.Services.Archive.Storage;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddSingleton<FilesStorage>()
    .AddHostedService<ArchiveMessageConsumer>()
    .AddRabbitMq(builder.Configuration)
    .AddMessageInbox();

var app = builder.Build();

app.MapGet("/", () => "Archive Service");

app.Run();

