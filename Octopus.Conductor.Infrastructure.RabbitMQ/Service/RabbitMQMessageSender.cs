using Microsoft.Extensions.Logging;
using Octopus.Conductor.Application.Interfaces;
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
    public class RabbitMQMessageSender : IMessageSender
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMQMessageSender> _logger;
        private readonly int _retryCount;
        private IModel _channel;
        private Policy _policy;

        public RabbitMQMessageSender(IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQMessageSender> logger,
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

        public Task SendMessage(object message)
        {
            if (!_persistentConnection.IsConnected)
            {
                if (_persistentConnection.TryConnect())
                    _channel = _persistentConnection.CreateModel();
            }
            
            var body = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType());

            _policy.Execute(() =>
            {
                var properties = _channel.CreateBasicProperties();
                IModel
                properties.DeliveryMode = 2;

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: "",
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });

            return Task.CompletedTask;
        }

        private IModel CreatePublisherChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: "test-exchange",
                                    type: "direct");

            channel.QueueDeclare(queue: "test-queue",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            return channel;
        }
    }
}
