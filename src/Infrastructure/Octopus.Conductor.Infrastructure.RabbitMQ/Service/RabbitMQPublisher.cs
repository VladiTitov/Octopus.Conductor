﻿using Microsoft.Extensions.Logging;
using Octopus.Conductor.Application.Constants;
using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text.Json;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Service
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly Interfaces.IConnection _persistentConnection;
        private readonly ILogger<RabbitMQPublisher> _logger;
        IModel _channel;
        Policy _policy;
        object _lock = new object();

        public RabbitMQPublisher(Interfaces.IConnection persistentConnection,
            ILogger<RabbitMQPublisher> logger)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _policy = CreatePolicy();
        }

        private Policy CreatePolicy() =>
            Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(RabbitMQConstants.RetryCount,
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
                properties.DeliveryMode = 2;

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
                if (_persistentConnection.IsConnected || _persistentConnection.TryConnect())
                    _channel = _persistentConnection.GetModel(channelName);
        }
    }
}
