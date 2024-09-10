using Filo.Services.Archive.Messaging;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHostedService<ArchiveMessageConsumer>()
    .AddRabbitMq(builder.Configuration)
    .AddMessageInbox();
var app = builder.Build();

app.MapGet("/", () => "Archive Service");
app.MapGet("/files", (IMessageInbox storage) => storage.GetAll());

app.Run();

