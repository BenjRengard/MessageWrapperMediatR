using MessageWrapperMediatR.Domain.Models;
using MessageWrapperMediatR.Infrastructure.IbmMqSeries.Publisher;
using MessageWrapperMediatR.Infrastructure.Kafka.Publisher;
using MessageWrapperMediatR.Infrastructure.Messaging;
using MessageWrapperMediatR.Infrastructure.RabbitMq.Publisher;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Infrastructure.MessageBus.Publisher
{
    public class GenericMessagePublisher : IGenericMessagePublisher
    {
        private readonly ILogger<GenericMessagePublisher> _logger;
        private readonly IRabbitMqPublisher _rabbitPublisher;
        private readonly IKafkaPublisher _kafkaPublisher;
        private readonly IMqSeriesPublisher _mqSeriesPublisher;
        private readonly PublisherConfig _publisherConfig;

        public GenericMessagePublisher(
            ILogger<GenericMessagePublisher> logger,
            PublisherConfig publisherConfig,
            IRabbitMqPublisher rabbitPublisher,
            IKafkaPublisher kafkaPublisher,
            IMqSeriesPublisher mqSeriesPublisher)
        {
            _logger = logger;
            _publisherConfig = publisherConfig;
            _rabbitPublisher = rabbitPublisher;
            _kafkaPublisher = kafkaPublisher;
            _mqSeriesPublisher = mqSeriesPublisher;
        }

        ///<inheritdoc/>
        public async Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeBase<T> message)
        {
            var ret = new PublisherResult();
            if (message != null)
            {
                List<PublisherMessageEnveloppeBase<T>> enveloppes = this.BuildEnveloppes(message);

                if (enveloppes.Count != 0)
                {
                    // Pour chaque enveloppes, on envoi via le bus spécifique de l'enveloppe. 
                    PublisherResult[] results = await Task.WhenAll(enveloppes.Select(e => this.DirectPublishToBusAsync(e)));
                    ret.IsSuccess = results.Any(x => x.IsSuccess == false) != true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrorMessage = $"Message non géré au niveau de la publication : {message.MessageType}";
                    _logger.LogError("Message non géré au niveau de la publication: {messageType}", message.MessageType);
                }
            }
            else
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = "Impossible d'envoyer un message, car l'enveloppe est vide.";
                _logger.LogError(ret.ErrorMessage);
            }
            return ret;
        }

        public async Task<PublisherResult> DirectPublishToBusAsync<T>(PublisherMessageEnveloppeBase<T> enveloppe)
        {
            var ret = new PublisherResult { IsSuccess = true };
            if (enveloppe is PublisherMessageEnveloppeRabbitMq<T> rabbitEnveloppe)
            {
                ret = await _rabbitPublisher.PublishMessageAsync(rabbitEnveloppe);
            }
            if (enveloppe is PublisherMessageEnveloppeKafka<T> kafkaEnveloppe)
            {
                ret = await _kafkaPublisher.PublishMessageAsync(kafkaEnveloppe);
            }
            if (enveloppe is PublisherMessageEnveloppeMqSeries<T> mqSeriesEnveloppe)
            {
                ret = await _mqSeriesPublisher.PublishMessageAsync(mqSeriesEnveloppe);
            }
            return ret;
        }

        public List<PublisherMessageEnveloppeBase<T>> BuildEnveloppes<T>(PublisherMessageEnveloppeBase<T> enveloppeBase)
        {
            var ret = new List<PublisherMessageEnveloppeBase<T>>();
            foreach (BusEndpointConfig endpointConfig in _publisherConfig.Endpoints.Where(x => x.Type == enveloppeBase.MessageType))
            {
                switch (endpointConfig.BusType)
                {
                    case MessageBusEnum.rabbitmq:
                        ret.Add(new PublisherMessageEnveloppeRabbitMq<T>(enveloppeBase.Message, enveloppeBase.MessageType, endpointConfig?.EndpointName, endpointConfig?.RoutingKey));
                        break;
                    case MessageBusEnum.kafka:
                        ret.Add(new PublisherMessageEnveloppeKafka<T>(enveloppeBase.Message, enveloppeBase.MessageType, endpointConfig?.EndpointName));
                        break;
                    case MessageBusEnum.ibmmqseries:
                        ret.Add(new PublisherMessageEnveloppeMqSeries<T>(enveloppeBase.Message, enveloppeBase.MessageType, endpointConfig?.EndpointName));
                        break;
                    default:
                        break;
                }
            }
            return ret;
        }

        public PublisherMessageEnveloppeBase<object> BuildEnveloppeFromMetadata(MessageBusEnum messageBus, string endpoint, string messageJson, string routingKey = null)
        {
            object anonymousObject = System.Text.Json.JsonSerializer.Serialize(messageJson);
            switch (messageBus)
            {
                case MessageBusEnum.rabbitmq:
                    return new PublisherMessageEnveloppeRabbitMq<object>(anonymousObject, string.Empty, endpoint, routingKey);
                case MessageBusEnum.kafka:
                    return new PublisherMessageEnveloppeKafka<object>(anonymousObject, string.Empty, endpoint);
                case MessageBusEnum.ibmmqseries:
                    return new PublisherMessageEnveloppeMqSeries<object>(anonymousObject, string.Empty, endpoint);
                default:
                    return null;
            }
        }
    }
}
