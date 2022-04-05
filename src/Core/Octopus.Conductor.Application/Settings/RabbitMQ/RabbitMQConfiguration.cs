using System.Collections.Generic;

namespace Octopus.Conductor.Application.Settings.RabbitMQ
{
    public class RabbitMQConfiguration
    {
        public Connection Connection { get; set; }
        public IDictionary<string, Channel> Channels { get; set; }
        public IDictionary<string, Publisher> Publishers { get; set; }
    }
}
