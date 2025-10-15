using IBM.XMS;
using MessageWrapperMediatR.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries.Publisher
{
    internal class MqSeriesPublisher : IMqSeriesPublisher, IDisposable
    {
        private const string BROKER_CC_DUR_SUBQ_VALUE = "SYSTEM.JMS.D.CC.SUBSCRIBER.QUEUE";

        private readonly SemaphoreSlim _semaphoreGate = new(1);
        private readonly ILogger<MqSeriesPublisher> _logger;
        private readonly MqSeriesConfig _mqSeriesConfig;

        #region Fields

        private bool disposedValue;

        public IConnection UniqueConnection { get; private set; }

        public ISession UniqueSession { get; private set; }

        #endregion

        public MqSeriesPublisher(ILogger<MqSeriesPublisher> logger, MqSeriesConfig mqSeriesConfig)
        {
            _logger = logger;
            _mqSeriesConfig = mqSeriesConfig;
            this.InitializeConnectionAndSession();
        }

        public async Task<PublisherResult> PublishMessageAsync<T>(PublisherMessageEnveloppeMqSeries<T> message)
        {
            await _semaphoreGate.WaitAsync();
            var ret = new PublisherResult { IsSuccess = false };
            if (_mqSeriesConfig.MqSeriesActivated)
            {


                if (message != null && !string.IsNullOrWhiteSpace(message.Queue))
                {
                    try
                    {
                        ret.MessageId = this.Publish(message);
                        ret.IsSuccess = !string.IsNullOrWhiteSpace(ret.MessageId);
                    }
                    catch (Exception exceptionFirstAttempt)
                    {
                        _logger.LogWarning(exceptionFirstAttempt,
                            "Error during sending message into MqSeries queue : {queueName}. Connection is maybe done. Retry seding.", message.Queue);
                        // Need to retry .
                        try
                        {
                            this.CloseAndDisposeConnection();
                            this.InitializeConnectionAndSession();
                            ret.MessageId = this.Publish(message);
                            ret.IsSuccess = !string.IsNullOrWhiteSpace(ret.MessageId);
                        }
                        catch (Exception exceptionSecondAttempt)
                        {
                            _logger.LogError(exceptionSecondAttempt, "Error during sending message into MqSeries");
                        }
                    }
                }
                else
                {
                    _logger.LogError("Impossible to send message of type {messageType} because no MqSeries queue is define in settings", message?.MessageType);
                }
            }
            else
            {
                _logger.LogError("Impossible d'envoyer le message {messageId}, car MqSeries n'est pas activé", message.MessageId);
            }
            _ = _semaphoreGate.Release();

            return ret;
        }

        private string Publish<T>(PublisherMessageEnveloppeMqSeries<T> message)
        {
            string messageId = string.Empty;
            if (this.UniqueConnection != null && this.UniqueSession != null)
            {
                using (IDestination destination = this.UniqueSession.CreateQueue(message.Queue))
                {
                    destination.SetIntProperty(XMSC.WMQ_MESSAGE_BODY, XMSC.WMQ_MESSAGE_BODY_MQ);
                    destination.SetIntProperty(XMSC.WMQ_TARGET_CLIENT, XMSC.WMQ_TARGET_DEST_MQ);
                    this.UniqueConnection.Start();
                    using (IMessageProducer producer = this.UniqueSession.CreateProducer(destination))
                    {
                        producer.DeliveryMode = DeliveryMode.NonPersistent;
                        string jsonMessage = System.Text.Json.JsonSerializer.Serialize(message.Message);
                        ITextMessage messageText = this.UniqueSession.CreateTextMessage(jsonMessage);
                        producer.Send(messageText);
                        messageId = this.FormatMessageId(messageText.JMSMessageID);
                        _logger.LogInformation("Sending Message {message} to queue {queueName}", messageText, message.Queue);
                        producer.Close();
                    }
                }
            }
            return messageId;
        }

        private void InitializeConnectionAndSession()
        {
            if (_mqSeriesConfig.MqSeriesActivated)
            {
                this.UniqueConnection = null;
                this.UniqueSession = null;
                this.UniqueConnection = this.TryCreateConnectionToMqSeriesWithXmsPackage();
                this.UniqueSession = this.UniqueConnection.CreateSession(false, AcknowledgeMode.AutoAcknowledge);
            }
        }

        private void CloseAndDisposeConnection()
        {
            try
            {
                this.UniqueConnection?.Close();
                this.UniqueSession?.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error during dispose of session");
            }
            try
            {
                this.UniqueSession?.Close();
                this.UniqueSession?.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error during dispose of connection");
            }
            this.UniqueSession = null;
            this.UniqueConnection = null;
        }

        /// <summary>
        /// Necessaire car le JWSMessageId starts with "ID:". 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        private string FormatMessageId(string messageId) => messageId.Substring(3);

        /// <summary>
        /// Create MQSeries connexion with TryCatch.
        /// </summary>
        /// <returns></returns>
        private IConnection TryCreateConnectionToMqSeriesWithXmsPackage()
        {
            IConnection connection = null;
            try
            {
                connection = this.CreateConnectionWithXmsPackage();
            }
            catch (XMSException xmsEx)
            {
                _logger.LogError(xmsEx, "CreateConnectionToMq - XMSException occurred during connection to MQ.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateConnectionToMq - Exception occurred during connection to MQ.");
            }
            return connection;
        }

        /// <summary>
        /// Create connexion to MqSeries with XMS package.
        /// </summary>
        /// <returns></returns>
        private IConnection CreateConnectionWithXmsPackage()
        {
            try
            {
                var xmfFactoryFactory = XMSFactoryFactory.GetInstance(XMSC.CT_WMQ);
                IConnectionFactory connectionFactory = xmfFactoryFactory.CreateConnectionFactory();
                connectionFactory.SetStringProperty(XMSC.WMQ_HOST_NAME, _mqSeriesConfig.ConnectionName);
                connectionFactory.SetIntProperty(XMSC.WMQ_PORT, _mqSeriesConfig.Port);
                connectionFactory.SetStringProperty(XMSC.WMQ_CHANNEL, _mqSeriesConfig.Channel);
                connectionFactory.SetIntProperty(XMSC.WMQ_CONNECTION_MODE, XMSC.WMQ_CM_CLIENT);
                connectionFactory.SetStringProperty(XMSC.WMQ_QUEUE_MANAGER, _mqSeriesConfig.QueueManagerName);
                connectionFactory.SetIntProperty(XMSC.WMQ_BROKER_VERSION, XMSC.WMQ_BROKER_V1);
                connectionFactory.SetIntProperty(XMSC.WMQ_STATUS_REFRESH_INTERVAL, _mqSeriesConfig.WaitInterval);
                connectionFactory.SetIntProperty(XMSC.WMQ_CLEANUP_INTERVAL, _mqSeriesConfig.CleanUpInterval);
                connectionFactory.SetStringProperty(XMSC.WMQ_BROKER_CC_DUR_SUBQ, BROKER_CC_DUR_SUBQ_VALUE);
                connectionFactory.SetStringProperty(XMSC.USERID, _mqSeriesConfig.User);
                connectionFactory.SetStringProperty(XMSC.PASSWORD, _mqSeriesConfig.Password);
                //connectionFactory.SetIntProperty(XMSC.WMQ_CLIENT_RECONNECT_OPTIONS, XMSC.WMQ_CLIENT_RECONNECT);
                //connectionFactory.SetIntProperty(XMSC.WMQ_CLIENT_RECONNECT_TIMEOUT, XMSC.WMQ_CLIENT_RECONNECT_TIMEOUT_DEFAULT);
                IConnection connection = connectionFactory.CreateConnection(_mqSeriesConfig.User, _mqSeriesConfig.Password);
                connection.ExceptionListener = new ExceptionListener((ex) => _logger.LogError(ex, "Exception MQSeries handling with Listener with message: {message}", ex.Message));
                return connection;
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError(ex, "CreateConnection - NotSupportedException occurred during connection initiation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateConnectionToMq - Exception occurred during connection to MQ.");
            }

            return null;
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.CloseAndDisposeConnection();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
