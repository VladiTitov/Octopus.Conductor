namespace Octopus.Conductor.Infrastructure.RabbitMQ.Interfaces
{
    public interface IMessagePublisher
    {
        void Publish<T>(T message, string channelName, string exchangeName, string routingKey);
    }
}
