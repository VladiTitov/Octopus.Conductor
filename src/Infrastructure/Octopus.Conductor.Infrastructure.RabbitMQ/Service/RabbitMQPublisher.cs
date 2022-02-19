using Microsoft.Extensions.Logging;
using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Service
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly IPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly int _retryCount;
        private IModel _channel;
        private Policy _policy;

        public RabbitMQPublisher(IPersistentConnection persistentConnection,
            ILogger<RabbitMQPublisher> logger,
            int retryCount)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _retryCount = retryCount;
            _policy = CreatePolicy();
        }

        private Policy CreatePolicy() =>
            RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex, "Could not publish message after {Timeout}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    });


        public void Publish<T>(T message, string channelName, string exchangeName, string routingKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                if (_persistentConnection.TryConnect())
                    _channel = _persistentConnection.GetModel(channelName);
            }

            var body = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType());

            _policy.Execute(() =>
            {
                var properties = _channel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }
    }
}
