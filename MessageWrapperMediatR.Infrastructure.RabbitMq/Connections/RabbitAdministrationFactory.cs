using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public class RabbitAdministrationFactory : RabbitConnectionFactoryBase, IRabbitAdministrationFactory
    {
        public List<string> ExchangesCache { get; set; } = new List<string>();

        public RabbitAdministrationFactory(ILogger<RabbitAdministrationFactory> logger, RabbitConfig rabbitConfig)
            : base(logger, rabbitConfig)
        { }

        ///<inheritdoc/>
        public bool RemoveExchangeInCache(string exchange)
        {
            return this.ExchangesCache.Remove(exchange);
        }

        ///<inheritdoc/>
        public void PurgeAllExchangeCache()
        {
            this.ExchangesCache.Clear();
        }

        ///<inheritdoc/>
        public async Task RegisterExchangeToPublishAsync(string exchange)
        {
            // If cache contains exchange, no need to recreate.
            if (!this.ExchangesCache.Contains(exchange))
            {
                await _semaphoreGate.WaitAsync();
                try
                {
                    using IConnection connection = await this.GetConnectionAsync(UserTypeEnum.admin);
                    using IChannel channel = await connection.CreateChannelAsync();
                    await channel.ExchangeDeclareAsync(exchange, "topic", true);
                    _logger.LogInformation("Exchange {exchange} is created", exchange);
                    await channel.CloseAsync();
                    await connection.CloseAsync();
                    this.ExchangesCache.Add(exchange);
                }
                catch
                {
                    _logger.LogError("Error during creation of exchange {exchange}", exchange);
                }
                finally
                {
                    _ = _semaphoreGate.Release();
                }
            }
        }

        ///<inheritdoc/>
        public async Task RegisterQueueAsync(string queue, List<(string routingKey, string exchange)> bindings)
        {
            await _semaphoreGate.WaitAsync();
            try
            {
                using IConnection connection = await this.GetConnectionAsync(UserTypeEnum.admin);
                using IChannel channel = await connection.CreateChannelAsync();
                _ = await channel.QueueDeclareAsync(queue, true, false, false);
                _logger.LogInformation("Queue {queue} is created", queue);
                foreach ((string routingKey, string exchange) in bindings)
                {
                    if (!this.ExchangesCache.Contains(exchange))
                    {
                        await channel.ExchangeDeclareAsync(exchange, "topic", true);
                        _logger.LogInformation("Exchange {exchange} is created", exchange);
                    }
                    await channel.QueueBindAsync(queue, exchange, routingKey);
                    _logger.LogInformation("Binding for exchange {exchange} and queue {queue} with the routingkey {routingkey} is created", exchange, queue, routingKey);
                }

                await channel.CloseAsync();
                await connection.CloseAsync();
            }
            catch
            {
                _logger.LogError("Error during creation of queue and bindings for {queue}", queue);
            }
            finally
            {
                _ = _semaphoreGate.Release();
            }
        }

    }
}
