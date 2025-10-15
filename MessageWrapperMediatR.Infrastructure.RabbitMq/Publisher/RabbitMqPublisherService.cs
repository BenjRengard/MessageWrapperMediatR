using MessageWrapperMediatR.Infrastructure.Messaging;
using MessageWrapperMediatR.Infrastructure.RabbitMq.Connections;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Publisher
{
    public class RabbitMqPublisherService : IRabbitMqPublisherService
    {
        private readonly ILogger<RabbitMqPublisherService> _logger;
        private readonly RabbitConfig _rabbitConfig;
        private readonly SemaphoreSlim _semaphoreGate = new(1);
        private readonly IRabbitAdministrationFactory _administrationFactory;
        private readonly IRabbitChannelPublishFactory _channelFactory;

        public RabbitMqPublisherService(
            ILogger<RabbitMqPublisherService> logger,
            RabbitConfig rabbitConfig,
            IRabbitAdministrationFactory rabbitAdministrationFactory,
            IRabbitChannelPublishFactory rabbitChannelPublishFactory)
        {
            _logger = logger;
            _rabbitConfig = rabbitConfig;
            _administrationFactory = rabbitAdministrationFactory;
            _channelFactory = rabbitChannelPublishFactory;
        }

        public async Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeRabbitMq<T> message)
        {
            var ret = new PublisherResult();
            if (message != null)
            {
                if (string.IsNullOrWhiteSpace(message.Exchange))
                {
                    message.Exchange = _rabbitConfig.ExchangeToPublishMsg;
                }
                await _semaphoreGate.WaitAsync();
                var channel = await _channelFactory.GetChannelAsync();
                ret.MessageId = message.MessageId;
                try
                {
                    await _administrationFactory.RegisterExchangeToPublishAsync(message.Exchange);
                    _logger.LogInformation("Start to publish message {messageId}", message.MessageId);
                    //On serialize en JSon
                    string jsonMessage = System.Text.Json.JsonSerializer.Serialize(message.Message);
                    byte[] encodedMessage = Encoding.UTF8.GetBytes(jsonMessage);
                    if (!string.IsNullOrWhiteSpace(message.Exchange) && encodedMessage != null)
                    {
                        var basicProps = new BasicProperties();
                        await channel.BasicPublishAsync(
                            exchange: message.Exchange,
                            routingKey: message.RoutingKey,
                            mandatory: false,
                            basicProperties: basicProps,
                            body: encodedMessage);
                        _logger.LogInformation("Finish to publish message {messageId}", message.MessageId);
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        _logger.LogError("Cannot publish a message without target exchange for message {messageId}", message?.MessageId);
                        ret.ErrorMessage = "Cannot publish a message without target exchange";
                    }
                }
                catch (Exception executionError)
                {
                    ret.IsSuccess = false;
                    ret.ErrorMessage = executionError.Message;
                    _logger.LogError("Error on publish message {messageId} for channel {channel} at: {time} : {message}. Error : {error}. Stack trace : {StackStrace}",
                        message.MessageId,
                        channel?.ChannelNumber ?? -1,
                        DateTimeOffset.Now,
                        executionError.Message,
                        executionError.Source,
                        executionError.StackTrace);
                    _administrationFactory.RemoveExchangeInCache(message.Exchange);
                }
                finally
                {
                    _ = _semaphoreGate.Release();
                }
            }
            else
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = "Message enveloppe is empty or exchange is not define. Cannot publish a message";
                _logger.LogError("Message enveloppe is empty exchange is not define. Cannot publish a message");
            }
            return ret;

        }

    }
}
