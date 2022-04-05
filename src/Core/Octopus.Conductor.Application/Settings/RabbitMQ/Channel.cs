using System.Collections.Generic;

namespace Octopus.Conductor.Application.Settings.RabbitMQ
{
    public class Channel
    {
        public IDictionary<string, Exchange> Exchanges { get; set; }
        public IDictionary<string, Queue> Queues { get; set; }
        public IEnumerable<Binding> Bindings { get; set; }
    }
}
