using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Connections
{
    public class RabbitChannelReceiveFactory : RabbitConnectionFactoryBase, IRabbitChannelReceiveFactory
    {
        private IConnection _connection;
        private readonly Dictionary<string, IChannel> _modelCache = new();
        private readonly IRabbitAdministrationFactory _rabbitAdministrationFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="rabbitConfig"></param>
        public RabbitChannelReceiveFactory(ILogger<RabbitChannelReceiveFactory> logger, RabbitConfig rabbitConfig, IRabbitAdministrationFactory rabbitAdministrationFactory)
            : base(logger, rabbitConfig)
        {
            _rabbitAdministrationFactory = rabbitAdministrationFactory;
            _connection = this.GetConnectionAsync(UserTypeEnum.consumer).GetAwaiter().GetResult();
        }

        ///<inheritdoc/>
        public async Task RestartConnectionAsync()
        {
            try
            {
                await _connection?.CloseAsync();
                _connection?.Dispose();
                _connection = null;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Trouble during restart of Rabbit Connection");
            }
            finally
            {
                try
                {
                    _connection = await this.GetConnectionAsync(UserTypeEnum.consumer);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error during restart of Rabbit Connection. App need to be restarted.");
                }
            }
        }

        ///<inheritdoc/>
        public async Task RegisterConsumptionAsync(
            string key,
            string queueName,
            Func<string, Task> executionMethod,
            List<(string routingKey, string exchange)> bindings,
            CancellationToken cancellationToken)
        {
            await _rabbitAdministrationFactory.RegisterQueueAsync(queueName, bindings);
            IChannel channel = await GetModelAsync(key, cancellationToken);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (obj, consumerArgs) => this.ConsumerReceivedAsync(consumerArgs, channel, executionMethod);
            _ = await channel.BasicConsumeAsync(queueName, false, consumer);
            _logger.LogInformation("Channel open for {key} in queue {queueName} with bindings", key, queueName);
        }

        ///<inheritdoc/>
        public async Task CancelConsumptionAsync(string key)
        {
            await _semaphoreGate.WaitAsync();
            try
            {
                if (_modelCache.TryGetValue(key, out IChannel channel) && channel != null)
                {
                    if (channel.IsOpen)
                    {
                        await channel.CloseAsync();
                    }
                    channel.Dispose();
                    _ = _modelCache.Remove(key);
                }
            }
            finally
            {
                _ = _semaphoreGate.Release();
            }
        }

        /// <summary>
        /// Wrapper for consumption of message.
        /// </summary>
        /// <param name="eventArg">Event arg.</param>
        /// <param name="channel">Model where the message is received.</param>
        /// <param name="executionMethod">Functionnal execution.</param>
        /// <returns></returns>
        private async Task ConsumerReceivedAsync(BasicDeliverEventArgs eventArg, IChannel channel, Func<string, Task> executionMethod)
        {
            try
            {
                ReadOnlyMemory<byte> fullBody = eventArg.Body;
                string message = Encoding.UTF8.GetString(fullBody.ToArray());
                await executionMethod.Invoke(message);
                await channel.BasicAckAsync(eventArg.DeliveryTag, false);
                _logger.LogInformation("Message consumed on channel {channel} at: {time} : {message}", channel.ChannelNumber, DateTimeOffset.Now, message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during consumption");
                await channel.BasicNackAsync(eventArg.DeliveryTag, false, false);
            }
        }

        /// <summary>
        /// Get a model.
        /// </summary>
        /// <param name="key">Key of model (channel).</param>
        /// <param name="cancellationToken">Toekn of cancellation.</param>
        /// <returns>Channel (Model) which be created.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<IChannel> GetModelAsync(string key, CancellationToken cancellationToken)
        {
            await _semaphoreGate.WaitAsync(cancellationToken);
            try
            {
                if (!_modelCache.TryGetValue(key, out IChannel channel) || channel.IsClosed)
                {
                    channel?.Dispose();
                    channel = await _connection.CreateChannelAsync();
                    if (channel != null)
                    {
                        await channel.BasicQosAsync(0, (ushort)_rabbitConfig.PrefectCount, false);
                        // Add Model
                        _modelCache[key] = channel;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unable to create a model for connection.");
                    }
                }

                return channel!;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get model for connection to handler {handlerKey}", key);
                throw new InvalidOperationException($"Unable to create a model for connection.");
            }
            finally
            {
                _ = _semaphoreGate.Release();
            }
        }
    }
}
