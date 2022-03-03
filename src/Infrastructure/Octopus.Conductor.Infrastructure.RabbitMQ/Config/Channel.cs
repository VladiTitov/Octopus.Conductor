using System.Collections.Generic;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Config
{
    public class Channel
    {
        public IDictionary<string, Exchange> Exchanges { get; set; }
        public IDictionary<string, Queue> Queues { get; set; }
        public IEnumerable<Binding> Bindings { get; set; }
    }
}
