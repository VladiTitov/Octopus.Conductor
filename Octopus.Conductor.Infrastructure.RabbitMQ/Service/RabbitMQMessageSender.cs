using Octopus.Conductor.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Service
{
    public class RabbitMQMessageSender : IMessageSender
    {
        public Task SendMessage(object message)
        {
            throw new NotImplementedException();
        }
    }
}
