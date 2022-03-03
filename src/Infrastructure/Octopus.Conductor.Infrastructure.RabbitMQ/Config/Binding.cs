using System.Collections.Generic;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Config
{
    public class Binding
    {
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
