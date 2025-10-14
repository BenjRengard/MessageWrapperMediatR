namespace MessageWrapperMediatR.Domain.Models
{
    /// <summary>
    /// Enumerable of supported message brokers.
    /// </summary>
    public enum MessageBusEnum
    {
        /// <summary>
        /// RabbitMq Message Bus.
        /// </summary>
        rabbitmq = 0,

        /// <summary>
        /// Kafka message bus.
        /// </summary>
        kafka = 1,

        /// <summary>
        /// IBM MQSeries Message Bus.
        /// </summary>
        ibmmqseries = 2
    }
}
