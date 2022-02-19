using RabbitMQ.Client;
using System;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces
{
    public interface IPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel GetModel(string channelName);
    }
}
