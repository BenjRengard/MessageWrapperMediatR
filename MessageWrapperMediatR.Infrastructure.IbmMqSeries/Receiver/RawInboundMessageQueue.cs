using IBM.WMQ;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries.Receiver
{
    public class RawInboundMessageQueue : IDisposable, ICancellable
    {
        private readonly ILogger _logger;

        private readonly string _qname;
        private readonly int _numConcurrentDequeueTasks;
        private readonly bool _useTransactions;
        private readonly int _numBackoutAttempts;

        private readonly Func<MQQueueManager> _mqQueueManagerFactory;
        private readonly Func<MQMessage> _messageFactory;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly MQGetMessageOptions _mqgmo;
        private readonly Func<string, Task> _executionMethod;

        private Task[] _dequeueTasks;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="qname"></param>
        /// <param name="executionMethod"></param>
        /// <param name="concurrentWorkers"></param>
        /// <param name="useTransactions"></param>
        /// <param name="numBackoutAttempts"></param>
        /// <param name="autoStart"></param>
        /// <param name="messageOptionsFactory"></param>
        /// <param name="messageFactory"></param>
        /// <param name="mqQueueManagerFactory"></param>
        public RawInboundMessageQueue(
            ILogger logger,
            string qname,
            Func<string, Task> executionMethod,
            int concurrentWorkers = 1,
            bool useTransactions = true,
            int numBackoutAttempts = 1,
            bool autoStart = true,
            Func<MQGetMessageOptions> messageOptionsFactory = null,
            Func<MQMessage> messageFactory = null,
            Func<MQQueueManager> mqQueueManagerFactory = null)
        {
            _logger = logger;
            _qname = qname;
            _executionMethod = executionMethod;
            _numConcurrentDequeueTasks = concurrentWorkers;
            _useTransactions = useTransactions;
            _numBackoutAttempts = numBackoutAttempts;

            _messageFactory = messageFactory ?? (() => new MQMessage());
            _mqgmo = messageOptionsFactory() ?? new MQGetMessageOptions();
            _mqQueueManagerFactory = mqQueueManagerFactory;

            if (autoStart)
            {
                this.Start();
            }
        }

        public void Start()
        {
            if (_dequeueTasks == null)
            {
                _logger.LogInformation("Starting {numConcurrentDequeueTasks} Dequeue tasks", _numConcurrentDequeueTasks);
                _dequeueTasks = new Task[_numConcurrentDequeueTasks];
                for (int i = 0; i < _numConcurrentDequeueTasks; i++)
                {
                    _dequeueTasks[i] = Task.Factory.StartNew(this.DequeueTaskAsync, i, _cancellationTokenSource.Token);
                }
            }
            else
            {
                _logger.LogWarning("Dequeue tasks have already started.");
            }
        }

        private async Task DequeueTaskAsync(object i)
        {
            int threadNum = (int)i;
#if DEBUG
            _logger.LogInformation(" Dequeue thread {ThreadNum} running on thread ({ManagedThreadId}: {ThreadName}) ", threadNum, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
#endif

            MQQueueManager queueManager = null;
            MQQueue queue = null;
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {

                if (queue == null || queueManager == null || !queueManager.IsConnected || !queue.IsOpen)
                {
                    queueManager = _mqQueueManagerFactory();

                    // exit the iteration If the connection fails
                    if (queueManager == null)
                    {
                        await Task.Delay(2000);
                        // exit only the current iteration
                        continue;
                    }

                    try
                    {
                        queue = queueManager.AccessQueue(_qname, MQC.MQOO_INPUT_AS_Q_DEF);
                    }
                    catch (Exception e)
                    {

                        _logger.LogError(e, "DequeueTask {threadNum}: Exception occurred during AccessQueue for {QueName}", threadNum, _qname);
                    }
                }

                try
                {
                    MQMessage message = _messageFactory();
                    bool commitOrBackoutRequired = false;

                    try
                    {
                        queue.Get(message, _mqgmo);
                        if (_useTransactions)
                        {
                            commitOrBackoutRequired = true;
                        }

                        int messageIdLength = message.MessageId.Length;
                        string messageId = messageIdLength > 8 ? BitConverter.ToString(message.MessageId, messageIdLength - 8, 8) : BitConverter.ToString(message.MessageId, 0, messageIdLength);

                        try
                        {
                            // Execute the method with the message in string.
                            await _executionMethod.Invoke(message.ReadString(message.MessageLength));

                            if (commitOrBackoutRequired)
                            {
                                queueManager.Commit();
                            }
                        }
                        catch (Exception subjectException)
                        {
                            _logger.LogError(subjectException, "DequeueTask {threadNum}: Exception occurred within the subject, proceed to commit/rollback/errorqueue phase ", threadNum);
                            try
                            {
                                if (commitOrBackoutRequired)
                                {
                                    if (message.BackoutCount < _numBackoutAttempts)
                                    {
                                        queueManager.Backout();
                                    }
                                    else
                                    {
                                        queueManager.Commit();
                                    }
                                }
                            }
                            catch (Exception exc2)
                            {
                                _logger.LogInformation(exc2, "DequeueTask {threadNum}: Exception occurred during commit/rollback/errorqueue phase", threadNum);
                            }
                        }

                    }
                    catch (MQException mqExc)
                    {
                        if (mqExc.Reason == 2033)
                        {
                            // message not available, ignore and micro-sleep 
                            await Task.Delay(100);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "DequeueTask {threadNum}: Exception occurred during dequeue task loop", threadNum);
                }
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_dequeueTasks);
            _dequeueTasks = null;
        }

        public bool IsStarted()
        {
            return this._dequeueTasks != null && _cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
