using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare("files-exchange", "direct", durable:true, autoDelete: false);

channel.QueueDeclare("archive-queue", durable: true, exclusive: false, autoDelete: false);
channel.QueueDeclare("security-queue", durable: true, exclusive: false, autoDelete: false);
channel.QueueDeclare("metadata-queue", durable: true, exclusive: false, autoDelete: false);

channel.QueueBind("archive-queue", "files-exchange", String.Empty);
channel.QueueBind("security-queue", "files-exchange", String.Empty);
channel.QueueBind("metadata-queue", "files-exchange", String.Empty);
