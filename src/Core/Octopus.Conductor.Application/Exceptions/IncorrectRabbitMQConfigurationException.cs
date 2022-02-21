using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Application.Exceptions
{
    public class IncorrectRabbitMQConfigurationException : Exception
    {
        public IncorrectRabbitMQConfigurationException(string msg)
            : base(msg) { }
    }
}
