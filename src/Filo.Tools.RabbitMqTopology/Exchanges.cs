namespace Filo.Tools.RabbitMqTopology;

public static class Exchanges
{
    public static class FilesExchange
    {
        public static string Default = "files-exchange";
        public static string ConsistentHash = "files-exchange-che";
    }
}