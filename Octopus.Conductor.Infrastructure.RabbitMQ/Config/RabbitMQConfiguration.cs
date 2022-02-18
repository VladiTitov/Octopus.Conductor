using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Config
{
    public class RabbitMQConfiguration
    {
        public Connection Connection { get; set; }
        public IDictionary<string, Channel> Channels { get; set; }
    }
}
