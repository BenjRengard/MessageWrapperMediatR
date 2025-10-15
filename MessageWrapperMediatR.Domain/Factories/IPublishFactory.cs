using MessageWrapperMediatR.Domain.Models;

namespace MessageWrapperMediatR.Domain.Factories
{
    /// <summary>
    /// Factory to publish message directly. 
    /// </summary>
    public interface IPublishFactory
    {
        /// <summary>
        /// Publish a message into a message bus.
        /// </summary>
        /// <param name="messageBus">Message bus target of publishing.</param>
        /// <param name="endpoint">Endpoint to publish. Queue in RabbitMq and IBM MQSeries purpose. Topic in Kafka purpose.</param>
        /// <param name="messageContentJson">Message to publish in json format.</param>
        /// <param name="routingKeyOptionnal">Optional routing key params in RabbitMq purpose.</param>
        /// <returns>True the message is sent. False the message failed to be sent.</returns>
        Task<bool> PublishMessageAsync(MessageBusEnum messageBus, string endpoint, string messageContentJson, string? routingKeyOptionnal = null);
    }
}
