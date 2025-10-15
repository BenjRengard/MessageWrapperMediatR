namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    /// <summary>
    /// Factory of channel for Rabbit Listening.
    /// </summary>
    public interface IRabbitChannelReceiveFactory
    {
        /// <summary>
        /// Cancel a channel and his consumption.
        /// </summary>
        /// <param name="key">Key of channel (Model) to shundown.</param>
        /// <returns>Task.</returns>
        Task CancelConsumptionAsync(string key);

        /// <summary>
        /// Register a channel and start the consumption.
        /// </summary>
        /// <param name="key">Key of channel to register.</param>
        /// <param name="queueName">Name of queue to listen.</param>
        /// <param name="executionMethod">Method to execute when message received.</param>
        /// <param name="cancellationToken">Token of cancellation.</param>
        /// <param name="exchange">Exchange whom provide message. To register the binding.</param>
        /// <param name="routingKey">Routing key of binding. To register the binding.</param>
        /// <returns>Task</returns>
        Task RegisterConsumptionAsync(string key, string queueName, Func<string, Task> executionMethod, List<(string routingKey, string exchange)> bindings, CancellationToken cancellationToken);

        /// <summary>
        /// Restart Rabbit Connection.
        /// </summary>
        Task RestartConnectionAsync();
    }
}
