using Microsoft.Extensions.Logging;
using Octopus.Conductor.Application.Constants;
using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text.Json;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Services
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IPersistanceConnection _connection;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private IModel _channel;
        private Policy _policy;
        private object _lock = new object();

        public RabbitMQPublisher(IPersistanceConnection connection,
            ILogger<RabbitMQPublisher> logger)
        {
            _connection = connection;
            _logger = logger;
            _policy = CreatePolicy();
        }

        private Policy CreatePolicy() =>
            Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                        {
                            _logger.LogWarning(ex,
                                "Could not publish message after {Timeout}s ({ExceptionMessage})",
                                $"{time.TotalSeconds:n1}", ex.Message);
                        });

        public void Publish<T>(T message, string channelName, string exchangeName, string routingKey)
        {
            TryConnectToChannel(channelName);

            var body = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType());

            _policy.Execute(() =>
            {
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

        private void TryConnectToChannel(string channelName)
        {
            lock (_lock)
                if (_connection.IsConnected || _connection.TryConnect())
                    _channel = _connection.GetModel(channelName);
        }
    }
}
