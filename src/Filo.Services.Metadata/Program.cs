using Filo.Services.Metadata.Messaging;
using Filo.Shared.Infrastructure.Messaging;
using Filo.Shared.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHostedService<MetadataMessageConsumer>()
    .AddRabbitMq(builder.Configuration)
    .AddMessageInbox();

var app = builder.Build();

app.MapGet("/", () => "Metadata Service");
app.MapGet("/files", (IMessageInbox storage) => storage.GetAll());

app.Run();