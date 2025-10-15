using Confluent.Kafka;
using MessageWrapperMediatR.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Infrastructure.Kafka.Publisher
{
    public class KafkaPublisher : IKafkaPublisher
    {
        private readonly ILogger _logger;
        private readonly KafkaConfig _kafkaConfig;

        private ProducerConfig _kafkaProducerConfig;

        public KafkaPublisher(ILogger<KafkaPublisher> logger, KafkaConfig kafkaConfig)
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig;
            this.InitializeProducerConfig();
        }

        public async Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeKafka<T> message)
        {
            var ret = new PublisherResult
            {
                MessageId = message.MessageId
            };
            if (_kafkaProducerConfig == null)
            {
                ret.IsSuccess = false;
                _logger.LogError("Impossible d'envoyer le message {messageId}. Problème de configuration dans le KafkaConfig pour le Producer", message.MessageId);
            }
            using (IProducer<Null, string> producer = new ProducerBuilder<Null, string>(_kafkaProducerConfig).Build())
            {
                try
                {
                    DeliveryResult<Null, string> result = await producer.ProduceAsync(message.Topic, new Message<Null, string>
                    {
                        Value = System.Text.Json.JsonSerializer.Serialize(message.Message)
                    });
                    if (result != null)
                    {
                        ret.IsSuccess = true;
                    }
                }
                catch (Exception produceException)
                {
                    _logger.LogError(produceException, "Error during publish message on Kafka on topic {topic} for message {messageId}", message.Topic, message.MessageId);
                    ret.IsSuccess = false;
                    ret.ErrorMessage = produceException.Message;
                }
            }
            return ret;
        }

        private void InitializeProducerConfig()
        {
            if (_kafkaConfig?.Producer != null)
            {
                _kafkaProducerConfig = new ProducerConfig
                {
                    BootstrapServers = _kafkaConfig.BootstrapServers,
                    ClientId = _kafkaConfig.Producer.ClientId
                };

                if (_kafkaConfig.SecurityEnable)
                {
                    _kafkaProducerConfig.SecurityProtocol = SecurityProtocol.Ssl;
                    _kafkaProducerConfig.SslCaLocation = _kafkaConfig.SslCaLocation;
                    _kafkaProducerConfig.SslKeystoreLocation = _kafkaConfig.Producer.SslKeystoreLocation;
                    _kafkaProducerConfig.SslKeystorePassword = _kafkaConfig.Producer.SslKeystorePassword;
                }
            }
            else
            {
                _logger.LogError("Problème de configuration dans le KafkaConfig pour le Producer");
            }
        }
    }
}
