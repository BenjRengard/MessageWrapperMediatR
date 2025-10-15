using Confluent.Kafka;
using MessageWrapperMediatR.Domain.Interfaces;
using MessageWrapperMediatR.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System.Text;

namespace MessageWrapperMediatR.Infrastructure.Kafka.Receiver
{
    public class KafkaGenericHandler : BackgroundService, IDynamicHandler
    {
        #region Fields

        private readonly ILogger<KafkaGenericHandler> _logger;
        private readonly Handler _handlerModel;
        private readonly KafkaConfig _kafkaConfig;
        private readonly Func<string, Task> _executionMethod;

        #endregion Fields

        ///<inheritdoc/>
        public MessageBusEnum BusType => MessageBusEnum.kafka;

        ///<inheritdoc/>
        public bool IsActive { get; private set; }

        ///<inheritdoc/>
        public string HandlerKey => _handlerModel.Id;

        ///<inheritdoc/>
        public string QueueFrom => _handlerModel.Queue;

        ///<inheritdoc/>
        public int TimeToLiveInDays => _handlerModel?.TimeToLiveInDays ?? 0;

        ///<inheritdoc/>
        public string AssociateCommand => _handlerModel.AssociateCommand;

        ///<inheritdoc/>
        public bool IsPermanent { get; set; } = false;

        ///<inheritdoc/>
        public List<Binding> Bindings => null;

        public KafkaGenericHandler(ILogger<KafkaGenericHandler> logger, Handler handler, KafkaConfig kafkaConfig, Func<string, Task> executionMethod)
        {
            _logger = logger;
            _handlerModel = handler;
            _kafkaConfig = kafkaConfig;
            _executionMethod = executionMethod;
        }

        #region Publics BackGroundService

        /// <summary>
        /// Override of Stop BackgroundService.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MessageEventHandler {topic} - Is stopping", this.HandlerKey);
            this.IsActive = false;
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Override of Start BackgroundService.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!this.IsActive && _kafkaConfig?.Consumer != null)
            {
                _logger.LogInformation("MessageEventHandler {topic} - Is starting", this.HandlerKey);
                this.IsActive = true;
                await base.StartAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Override of dispose.
        /// </summary>
        public override void Dispose()
        {
            this.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            base.Dispose();
        }

        #endregion

        #region Protected

        /// <summary>
        /// Override of ExecuteAsync BackgroundService.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_kafkaConfig?.Consumer != null)
            {
                _logger.LogInformation("MessageEventHandler {topic} - Is starting", this.HandlerKey);
                _ = Task.Run(async () =>
                {
                    while (this.IsActive)
                    {
                        await this.StartAsync();
                        _logger.LogInformation("Re start of Kafka consummer for {topic}", this.HandlerKey);
                    }
                }, stoppingToken);
            }
            return Task.CompletedTask;
        }

        #endregion

        #region Privates

        /// Suppression de règle Pragma pour permettre de garder le thread ouvert.
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task StartAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cancellationTokenSource.Cancel();
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                GroupId = _kafkaConfig.Consumer.GroupId,
                AutoOffsetReset = _kafkaConfig.AutoOffsetReset,
                EnableAutoCommit = _kafkaConfig.EnableAutoCommit
            };

            if (_kafkaConfig.SecurityEnable)
            {
                consumerConfig.SecurityProtocol = SecurityProtocol.Ssl;
                consumerConfig.SslCaLocation = _kafkaConfig.SslCaLocation;
                consumerConfig.SslKeystoreLocation = _kafkaConfig.Consumer.SslKeystoreLocation;
                consumerConfig.SslKeystorePassword = _kafkaConfig.Consumer.SslKeystorePassword;
            }
            try
            {
                using IConsumer<string, byte[]> consumer = new ConsumerBuilder<string, byte[]>(consumerConfig).Build();
                consumer.Subscribe(this.QueueFrom);
                _logger.LogInformation("Kafka handler for topic {topic} running", this.HandlerKey);
                _logger.LogInformation("Consumer configuration - Servers: {servers} | Topics: {topic} | GroupId: {groupId}",
                consumerConfig.BootstrapServers, this.QueueFrom, _kafkaConfig.Consumer.GroupId);
                bool isError = false;
                while (!isError && this.IsActive)
                {
                    isError = Policy
                    .HandleResult(true)
                    .WaitAndRetry(
                        5,
                        retryAttempt => TimeSpan.FromSeconds(1),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning("Retry number {retryCount} for Kafka consummer.", retryCount);
                        }).Execute(() =>
                        {
                            return this.Execute(cancellationTokenSource, consumer).GetAwaiter().GetResult();
                        });
                }
                consumer.Close();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Impossible de lancer le handler Kafka pour le topic {topic}", this.QueueFrom);
                this.IsActive = false;
            }
        }

        private async Task<bool> Execute(CancellationTokenSource cancellationTokenSource, IConsumer<string, byte[]> consumer)
        {
            bool relunchActive = false;
            try
            {
                await this.ContructConsumer(cancellationTokenSource, consumer);
            }
            catch (ConsumeException e)
            {
                _logger.LogError("Consume error: {error}", e.Error.Reason);
                relunchActive = true;
            }
            catch (KafkaException e)
            {
                _logger.LogError(e, "Kafka error: {error}", e.Error.Reason);
                relunchActive = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred on consumer handler Method.");
                relunchActive = true;
            }
            return relunchActive;
        }

        private async Task ContructConsumer(CancellationTokenSource cts, IConsumer<string, byte[]> consumer)
        {
            ConsumeResult<string, byte[]> consumeResult = consumer.Consume(cts.Token);
            string message = Encoding.UTF8.GetString(consumeResult.Message.Value);
            await _executionMethod.Invoke(message);
            _logger.LogDebug("Message consumed in topic {topic} at: {time} with offset : {offset}",
                consumeResult.Topic,
                DateTimeOffset.Now,
                consumeResult.Offset);
            _ = consumer.Commit();

        }

        #endregion
    }
}
