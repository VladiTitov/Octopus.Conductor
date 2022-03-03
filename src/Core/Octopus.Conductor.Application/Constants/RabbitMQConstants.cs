namespace Octopus.Conductor.Application.Constants
{
    public static class RabbitMQConstants
    {
        public const int RetryCount = 5;

        public const string FolderListnerChannelName = "demo-channel";

        public const string FolderListnerExchangeName = "demo-exchange";

        public const string FolderListnerRoutingKey = "demo-key";
    }
}
