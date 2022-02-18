using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Config
{
    public class Channel
    {
        public IDictionary<string, Exchange> Exchanges { get; set; }
        public IDictionary<string, Queue> Queues { get; set; }
        public IEnumerable<Binding> Bindings { get; set; }
    }
}
