using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octopus.Conductor.Application.Constants;
using Octopus.Conductor.Application.Exceptions;
using Octopus.Conductor.Infrastructure.RabbitMQ.Config;
using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Octopus.Conductor.Infrastructure.RabbitMQ.Services
{
    public class RabbitMQConnection : IPersistanceConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMQConfiguration _configuration;
        private readonly ILogger<RabbitMQConnection> _logger;
        private IDictionary<string, IModel> _channels;
        private Policy _policy;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(
            ILogger<RabbitMQConnection> logger,
            IOptions<RabbitMQConfiguration> configuration)
        {
            _configuration = configuration.Value;

            _connectionFactory = new ConnectionFactory
            {
                HostName = _configuration.Connection.Hostname,
                Port = _configuration.Connection.Port,
                UserName = _configuration.Connection.UserName,
                Password = _configuration.Connection.Password,
                VirtualHost = _configuration.Connection.VirtualHost,
            };

            _logger = logger;
            _policy = CreatePolicy();
            _channels = new Dictionary<string, IModel>();
        }

        private Policy CreatePolicy() =>
            Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(RabbitMQConstants.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex,
                                "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",
                                $"{time.TotalSeconds:n1}", ex.Message);
                    });

        public bool IsConnected =>
            _connection != null && _connection.IsOpen && !_disposed;

        public IModel GetModel(string channelName)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException(
                    "No RabbitMQ connections are available to perform this action");
            }

            if (!_configuration.Channels.ContainsKey(channelName))
            {
                throw new IncorrectRabbitMQConfigurationException(
                    $"Configuration file doesn't contain channel with name: {channelName}");
            }

            if (_channels.ContainsKey(channelName))
                return _channels[channelName];

            var channel = _connection.CreateModel();
            var channelConf = _configuration.Channels[channelName];

            DeclareExchanges(channel, channelConf);
            DeclareQueues(channel, channelConf);
            DeclareBinds(channel, channelConf);

            return channel;
        }

        private void DeclareExchanges(IModel channel, Channel channelConfig)
        {
            foreach (var exch in channelConfig.Exchanges)
            {
                channel.ExchangeDeclare(
                    exchange: exch.Key,
                    type: exch.Value.Type,
                    durable: exch.Value.Durable,
                    autoDelete: exch.Value.AutoDelete,
                    arguments: exch.Value.Arguments);
            }
        }
        private void DeclareQueues(IModel channel, Channel channelConfig)
        {
            foreach (var queue in channelConfig.Queues)
            {
                channel.QueueDeclare(
                    queue: queue.Key,
                    durable: queue.Value.Durable,
                    exclusive: queue.Value.Exclusive,
                    autoDelete: queue.Value.AutoDelete,
                    arguments: queue.Value.Arguments);
            }
        }
        private void DeclareBinds(IModel channel, Channel channelConfig)
        {
            foreach (var bind in channelConfig.Bindings)
            {
                channel.QueueBind(
                    queue: bind.Queue,
                    exchange: bind.Exchange,
                    routingKey: bind.RoutingKey,
                    arguments: bind.Arguments);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.ConnectionShutdown -= OnConnectionShutdown;
                _connection.CallbackException -= OnCallbackException;
                _connection.ConnectionBlocked -= OnConnectionBlocked;
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
            finally
            {
                _logger.LogWarning("RabbitMQ connection is closed");
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            _policy.Execute(() =>
            {
                _connection = _connectionFactory
                        .CreateConnection();
            });

            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;

                _logger.LogInformation(
                    "RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events"
                    , _connection.Endpoint.HostName);

                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
    }
}
