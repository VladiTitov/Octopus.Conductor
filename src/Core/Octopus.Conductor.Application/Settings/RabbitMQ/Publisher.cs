namespace Octopus.Conductor.Application.Settings.RabbitMQ
{
    public class Publisher
    {
        public string Channel { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}
