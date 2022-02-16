using RabbitMQ.Client;
using System;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}
