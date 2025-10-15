using MessageWrapperMediatR.Domain.Interfaces;
using MessageWrapperMediatR.Domain.Models;
using MessageWrapperMediatR.Infrastructure.RabbitMq.Connections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Infrastructure.RabbitMq.Receiver
{
    public class RabbitGenericHandler : BackgroundService, IDynamicHandler
    {
        #region Fields

        private readonly ILogger<RabbitGenericHandler> _logger;
        private readonly Handler _handlerModel;
        private readonly IRabbitChannelReceiveFactory _channelFactory;
        private readonly Func<string, Task> _executionMethod;

        #endregion Fields

        #region Properties

        ///<inheritdoc/>
        public MessageBusEnum BusType => MessageBusEnum.rabbitmq;

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
        public List<Binding> Bindings => _handlerModel?.Bindings ?? new List<Binding>();

        #endregion Properties

        public RabbitGenericHandler(
            ILogger<RabbitGenericHandler> logger,
            Handler handler,
            IRabbitChannelReceiveFactory channelFactory,
            Func<string, Task> executionMethod)
        {
            _handlerModel = handler;
            _logger = logger;
            _channelFactory = channelFactory;
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
            _logger.LogInformation("{name} - Is stopping", _handlerModel.Id);
            this.IsActive = false;
            await _channelFactory.CancelConsumptionAsync(this.HandlerKey);
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Override of Start BackgroundService.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!this.IsActive)
            {
                _logger.LogInformation("{name} - Is starting", _handlerModel.Id);
                List<(string routingKey, string exchange)> bindings = _handlerModel.Bindings
                                                                     .Select(b => (b.RoutingKey, b.Exchange))
                                                                     .ToList();
                try
                {
                    await _channelFactory.RegisterConsumptionAsync(this.HandlerKey, _handlerModel.Queue, _executionMethod, bindings, cancellationToken);
                    this.IsActive = true;
                    await base.StartAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error during starting of handler {hanlder} with error {messageError}", _handlerModel.Id, e.Message);
                    await this.StopAsync(cancellationToken);
                }
            }
        }

        /// <summary>
        /// Override of dispose.
        /// </summary>
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize => It's call by the base.Dispose();

        public override void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize => It's call by the base.Dispose();
        {
            this.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            base.Dispose();
        }

        #endregion Publics BackGroundService

        #region Protected

        /// <summary>
        /// Override of ExecuteAsync BackgroundService.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

        #endregion Protected
    }
}
