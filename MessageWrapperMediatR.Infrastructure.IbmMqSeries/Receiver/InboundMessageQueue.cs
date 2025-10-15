using IBM.WMQ;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries.Receiver
{
    public class InboundMessageQueue
    {
        private readonly ILogger _logger;

        private readonly Hashtable _connectionProperties;
        private readonly MqSeriesConfig _mqSeriesConfig;
        private readonly bool _useTransactions;

        private readonly RawInboundMessageQueue _underlying;

        private static MQQueueManager _mQQueueManager;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mqSeriesConfig"></param>
        /// <param name="queueInfo"></param>
        /// <param name="executionMethod"></param>
        /// <param name="adapter"></param>
        /// <param name="concurrentWorkers"></param>
        /// <param name="useTransactions"></param>
        /// <param name="numBackoutAttempts"></param>
        /// <param name="autoStart"></param>
        public InboundMessageQueue(
            ILogger logger,
            MqSeriesConfig mqSeriesConfig,
            string queueInfo,
            Func<string, Task> executionMethod,
            int concurrentWorkers = 1,
            bool useTransactions = true,
            int numBackoutAttempts = 1,
            bool autoStart = true)
        {
            _logger = logger;
            _mqSeriesConfig = mqSeriesConfig;
            _useTransactions = useTransactions;
            _connectionProperties = this.CreateConnectionProperties(_mqSeriesConfig);

            _underlying = new RawInboundMessageQueue(
                logger,
                queueInfo,
                executionMethod,
                concurrentWorkers,
                useTransactions,
                numBackoutAttempts,
                autoStart,
                this.NewMessageOptions,
                this.NewMessage,
                this.NewQueueManager);
        }

        public void Start()
        {
            _underlying.Start();
        }
        public bool IsStarted()
        {
            return _underlying.IsStarted();
        }

        public void Cancel()
        {
            _underlying.Cancel();
        }

        private MQMessage NewMessage()
        {
            var mqMessage = new MQMessage();
            return mqMessage;
        }

        private MQGetMessageOptions NewMessageOptions()
        {
            var mqgmo = new MQGetMessageOptions
            {
                Options = 0
            };
            if (_useTransactions)
            {
                mqgmo.Options |= MQC.MQGMO_SYNCPOINT;
            }
            mqgmo.Options |= MQC.MQGMO_WAIT;
            mqgmo.Options |= MQC.MQGMO_FAIL_IF_QUIESCING;
            mqgmo.Options |= MQC.MQGMO_CONVERT;
            return mqgmo;
        }

        private MQQueueManager NewQueueManager()
        {
            try
            {
                if (_mQQueueManager != null && !_mQQueueManager.IsConnected)
                {
                    _mQQueueManager.Connect();
                }
                else
                {
                    _mQQueueManager = new MQQueueManager(_mqSeriesConfig.QueueManagerName, _connectionProperties);
                }

                return _mQQueueManager;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection to Queue manager failed with {message}", ex.Message);
            }
            return null;
        }

        public Hashtable CreateConnectionProperties(MqSeriesConfig mqSeriesConfig)
        {
            var connectionProperties = new Hashtable();
            if (!string.IsNullOrEmpty(mqSeriesConfig.ConnectionName))
            {
                connectionProperties.Add(MQC.HOST_NAME_PROPERTY, mqSeriesConfig.ConnectionName);
            }

            if (!string.IsNullOrEmpty(mqSeriesConfig.Channel))
            {
                connectionProperties.Add(MQC.CHANNEL_PROPERTY, mqSeriesConfig.Channel);
            }

            connectionProperties.Add(MQC.PORT_PROPERTY, mqSeriesConfig.Port);

            if (!string.IsNullOrEmpty(mqSeriesConfig.User) && !string.IsNullOrEmpty(mqSeriesConfig.Password))
            {
                connectionProperties.Add(MQC.USER_ID_PROPERTY, mqSeriesConfig.User);
                connectionProperties.Add(MQC.PASSWORD_PROPERTY, mqSeriesConfig.Password);
                connectionProperties.Add(MQC.USE_MQCSP_AUTHENTICATION_PROPERTY, true);
            }

            connectionProperties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED);
            return connectionProperties;
        }
    }
}
