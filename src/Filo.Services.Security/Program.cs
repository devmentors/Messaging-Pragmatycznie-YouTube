using Filo.Services.Security.Messaging;
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

app.Run();