using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using System;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Service
{
    public class RabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        public bool IsConnected => throw new NotImplementedException();

        public IModel CreateModel()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryConnect()
        {
            throw new NotImplementedException();
        }
    }
}
