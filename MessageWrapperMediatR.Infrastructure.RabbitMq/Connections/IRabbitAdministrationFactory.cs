namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    /// <summary>
    /// Intarface of factory of connection with admin user in rabbitMq.
    /// </summary>
    public interface IRabbitAdministrationFactory
    {
        /// <summary>
        /// Register and create if not created queue in rabbitMq Virtual Host.
        /// </summary>
        /// <param name="queue">Name of queue</param>
        /// <param name="bindings">Bindins of queue. List of tuples routingkey / exchange.</param>
        /// <returns>Task void.</returns>
        Task RegisterQueueAsync(string queue, List<(string routingKey, string exchange)> bindings);

        /// <summary>
        /// Register and create if not created exchange in RabbitMq Virtual Host. 
        /// </summary>
        /// <param name="exchange">Name of exchange to create.</param>
        /// <returns>Task void.</returns>
        Task RegisterExchangeToPublishAsync(string exchange);

        /// <summary>
        /// Delete exchange in cache by name of exchange.
        /// </summary>
        /// <param name="exchange">Name of exchange to delete in cache.</param>
        /// <returns>True </returns>
        bool RemoveExchangeInCache(string exchange);

        /// <summary>
        /// Purge cache of exchanges.
        /// </summary>
        void PurgeAllExchangeCache();
    }
}
