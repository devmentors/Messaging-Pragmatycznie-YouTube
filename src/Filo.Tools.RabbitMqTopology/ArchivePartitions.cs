namespace Filo.Tools.RabbitMqTopology;

public static class ArchivePartitions
{
    public static class FilesExchange
    {
        public static int PartitionCount = 2;

        public static string GetQueueNameForPartitionNum(int partitionNum)
            => $"archive-queue-partition-{partitionNum}";
    }
}