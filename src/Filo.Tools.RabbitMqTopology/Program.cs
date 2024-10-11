using Filo.Tools.RabbitMqTopology;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// channel.ExchangeDeclare(Exchanges.FilesExchange.Default, "direct", durable:true, autoDelete: false);
//
// channel.QueueDeclare(Queues.Archive, durable: true, exclusive: false, autoDelete: false);
// channel.QueueDeclare("security-queue", durable: true, exclusive: false, autoDelete: false);
// channel.QueueDeclare("metadata-queue", durable: true, exclusive: false, autoDelete: false);
//
// channel.QueueBind(Queues.Archive, Exchanges.FilesExchange.Default, String.Empty);
// channel.QueueBind("security-queue", Exchanges.FilesExchange.Default, String.Empty);
// channel.QueueBind("metadata-queue", Exchanges.FilesExchange.Default, String.Empty);

channel.ExchangeDeclare(Exchanges.FilesExchange.ConsistentHash, "x-consistent-hash",durable: true, autoDelete: false);
foreach (var num in Enumerable.Range(1, ArchivePartitions.FilesExchange.PartitionCount))
{
    channel.QueueDeclare(ArchivePartitions.FilesExchange.GetQueueNameForPartitionNum(num), durable: true, exclusive: false, autoDelete: false);
    channel.QueueBind(
        ArchivePartitions.FilesExchange.GetQueueNameForPartitionNum(num), 
        Exchanges.FilesExchange.ConsistentHash,
        /* This is actually weight; we want each partition to get (proportionally) equal amount of messages, so all get 1 */
        "1");
}