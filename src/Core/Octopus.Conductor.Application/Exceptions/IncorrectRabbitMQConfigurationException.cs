using System;

namespace Octopus.Conductor.Application.Exceptions
{
    public class IncorrectRabbitMQConfigurationException : Exception
    {
        public IncorrectRabbitMQConfigurationException(string msg)
            : base(msg) { }
    }
}
