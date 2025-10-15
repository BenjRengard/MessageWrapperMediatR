using MessageWrapperMediatR.Domain.Factories;
using MessageWrapperMediatR.Domain.Interfaces;
using MessageWrapperMediatR.Domain.Models;
using MessageWrapperMediatR.Infrastructure.IbmMqSeries;
using MessageWrapperMediatR.Infrastructure.IbmMqSeries.Receiver;
using MessageWrapperMediatR.Infrastructure.Kafka;
using MessageWrapperMediatR.Infrastructure.Kafka.Receiver;
using MessageWrapperMediatR.Infrastructure.RabbitMq.Connections;
using MessageWrapperMediatR.Infrastructure.RabbitMq.Receiver;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Infrastructure.MessageBus.AutomaticHandlers
{
    ///<inheritdoc/>
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IRabbitChannelReceiveFactory _rabbitChannelReceiveFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaConfig _kafkaConfig;
        private readonly MqSeriesConfig _mqSeriesConfig;

        public HandlerFactory(
            IRabbitChannelReceiveFactory rabbitChannelReceiveFactory,
            IServiceProvider serviceProvider,
            KafkaConfig kafkaConfig,
            MqSeriesConfig mqSeriesConfig)
        {
            _rabbitChannelReceiveFactory = rabbitChannelReceiveFactory;
            _serviceProvider = serviceProvider;
            _kafkaConfig = kafkaConfig;
            _mqSeriesConfig = mqSeriesConfig;
        }

        ///<inheritdoc/>
        public IDynamicHandler CreateHandler(Handler handlerDefinition, Func<string, Task> executionMethod)
        {
            switch (handlerDefinition.BusType)
            {
                case MessageBusEnum.rabbitmq:
                    return new RabbitGenericHandler(this.GetLogger<RabbitGenericHandler>(), handlerDefinition, _rabbitChannelReceiveFactory, executionMethod);
                case MessageBusEnum.kafka:
                    return new KafkaGenericHandler(this.GetLogger<KafkaGenericHandler>(), handlerDefinition, _kafkaConfig, executionMethod);
                case MessageBusEnum.ibmmqseries:
                    return new MqSeriesGenericHandler(this.GetLogger<MqSeriesGenericHandler>(), handlerDefinition, _mqSeriesConfig, executionMethod);
                default:
                    return null;
            }
        }

        ///<inheritdoc/>
        public async Task RestartConnectionsOfHandlersAsync() => await _rabbitChannelReceiveFactory.RestartConnectionAsync();

        /// <summary>
        /// Get logger.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ILogger<T> GetLogger<T>()
        {
            return _serviceProvider.GetService(typeof(ILogger<T>)) as ILogger<T>;
        }
    }
}
