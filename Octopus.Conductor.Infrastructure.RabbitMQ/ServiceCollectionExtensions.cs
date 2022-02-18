using Microsoft.Extensions.DependencyInjection;
using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using Octopus.Conductor.Infrastructure.RabbitMQ.Service;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Conductor.Infrastructure.RabbitMQ
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRabbitMQConnection(this IServiceCollection services)
        {
            services.AddSingleton<IPersistentConnection, RabbitMQPersistentConnection>();
        }

        public static void AddRabbitMQPublisher(this IServiceCollection services)
        {
            services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
        }

        public static void AddConnectionFactory(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionFactory,ConnectionFactory>();
        }
    }
}
