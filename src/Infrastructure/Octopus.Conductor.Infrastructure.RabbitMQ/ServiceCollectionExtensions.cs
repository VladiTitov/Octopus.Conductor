using Microsoft.Extensions.DependencyInjection;
using Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces;
using Octopus.Conductor.Infrastructure.RabbitMQ.Service;

namespace Octopus.Conductor.Infrastructure.RabbitMQ
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRabbitMQConnection(this IServiceCollection services)
        {
            services.AddScoped<IPersistanceConnection, RabbitMQConnection>();
        }

        public static void AddRabbitMQPublisher(this IServiceCollection services)
        {
            services.AddScoped<IMessagePublisher, RabbitMQPublisher>();
        }
    }
}
