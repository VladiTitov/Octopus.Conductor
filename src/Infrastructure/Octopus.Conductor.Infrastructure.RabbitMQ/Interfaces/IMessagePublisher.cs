using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces
{
    public interface IMessagePublisher
    {
        void Publish<T> (T message,string channelName, string exchangeName, string routingKey);
    }
}
