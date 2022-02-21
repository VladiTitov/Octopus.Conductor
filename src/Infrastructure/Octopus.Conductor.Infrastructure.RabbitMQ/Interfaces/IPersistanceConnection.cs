using RabbitMQ.Client;
using System;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces
{
    public interface IPersistanceConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel GetModel(string channelName);
    }
}
