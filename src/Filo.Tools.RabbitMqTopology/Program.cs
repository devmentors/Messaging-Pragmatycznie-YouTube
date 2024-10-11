using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// channel.ExchangeDeclare("file-uploaded-exchange", "direct", durable:true, autoDelete: false);
// channel.ExchangeDeclare("file-renamed-exchange", "direct", durable:true, autoDelete: false);
//
// channel.QueueDeclare("file-uploaded-queue", durable: true, exclusive: false, autoDelete: false);
// channel.QueueDeclare("file-renamed-queue", durable: true, exclusive: false, autoDelete: false);
//
// channel.QueueBind("file-uploaded-queue", "file-uploaded-exchange", String.Empty);
// channel.QueueBind("file-renamed-queue", "file-renamed-exchange", String.Empty);
//
// var arguments = new Dictionary<string, object>
// {
//     {"x-single-active-consumer", true}
// };

channel.ExchangeDeclare("files-exchange", "direct", durable:true, autoDelete: false);
channel.QueueDeclare("archive-queue", durable: true, exclusive: false, autoDelete: false);
channel.QueueBind("archive-queue", "files-exchange", String.Empty);
