using MessageWrapperMediatR.Domain.Interfaces;
using MessageWrapperMediatR.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.IbmMqSeries.Receiver
{
    public class MqSeriesGenericHandler : BackgroundService, IDynamicHandler
    {
        private readonly ILogger<MqSeriesGenericHandler> _logger;
        private readonly Handler _handlerModel;
        private readonly Func<string, Task> _executionMethod;
        private readonly MqSeriesConfig _mqSeriesConfig;

        /// <summary>
        /// InboundMessage queue.
        /// </summary>
        public InboundMessageQueue Inbound { get; private set; }

        public MessageBusEnum BusType => MessageBusEnum.ibmmqseries;

        ///<inheritdoc/>
        public bool IsActive { get; private set; }

        ///<inheritdoc/>
        public string HandlerKey => _handlerModel?.Id;

        ///<inheritdoc/>
        public string QueueFrom => _handlerModel?.Queue;

        ///<inheritdoc/>
        public int TimeToLiveInDays => _handlerModel?.TimeToLiveInDays ?? 0;

        ///<inheritdoc/>
        public string AssociateCommand => _handlerModel?.AssociateCommand;

        ///<inheritdoc/>
        public bool IsPermanent { get; set; } = false;

        ///<inheritdoc/>
        public List<Binding> Bindings => _handlerModel?.Bindings ?? new List<Binding>();

        public MqSeriesGenericHandler(
            ILogger<MqSeriesGenericHandler> logger,
            Handler handler,
            MqSeriesConfig mqSeriesConfig,
            Func<string, Task> executionMethod)
        {
            _logger = logger;
            _handlerModel = handler;
            _executionMethod = executionMethod;
            _mqSeriesConfig = mqSeriesConfig;
        }

        #region Publics BackGroundService

        /// <summary>
        /// Override of Start BackgroundService.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!this.IsActive && _mqSeriesConfig.MqSeriesActivated)
            {
                _logger.LogInformation("{name} - Is starting", _handlerModel.Id);
                this.IsActive = true;
                await base.StartAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Override of Stop BackgroundService.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.IsActive)
            {
                _logger.LogInformation("{name} - Is stopping", _handlerModel.Id);
                if (this.Inbound != null && this.Inbound.IsStarted())
                {
                    this.Inbound.Cancel();
                }
                this.Inbound = null;
                this.IsActive = false;
                await base.StopAsync(cancellationToken);
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
            this.Inbound = new InboundMessageQueue(_logger, _mqSeriesConfig, _handlerModel.Queue, _executionMethod, autoStart: false);
            this.Inbound.Start();
            return Task.CompletedTask;
        }

        #endregion
    }
}
