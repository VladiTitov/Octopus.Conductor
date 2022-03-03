using System.Collections.Generic;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Config
{
    public class RabbitMQConfiguration
    {
        public Connection Connection { get; set; }
        public IDictionary<string, Channel> Channels { get; set; }
    }
}
