using AutoMapper;
using MediatR;
using MessageWrapperMediatR.Application.Commands.MessageCollector;
using MessageWrapperMediatR.Core.Factories;
using MessageWrapperMediatR.Core.Filters;
using MessageWrapperMediatR.Core.Interfaces;
using MessageWrapperMediatR.Core.Models;
using MessageWrapperMediatR.Core.Repositories;
using MessageWrapperMediatR.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageWrapperMediatR.Application.Services
{
    public class HandlersService : IHandlersService, IDisposable
    {
        #region Fields

        private readonly ILogger<HandlersService> _logger;
        private readonly IHandlerRepository _handlerRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerFactory _handlerFactory;
        private bool disposedValue;
        private readonly IMapper _mapper;
        private readonly PermanentHandlersConfig _permanentHandlersConfig;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Registered handlers.
        /// </summary>
        public Dictionary<string, IDynamicHandler> RegisteredHandlers { get; private set; } = new Dictionary<string, IDynamicHandler>();

        /// <summary>
        /// Task of initialization of Handlers.
        /// </summary>
        public Task InitTaskAsynchronous { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="handlerRepository"></param>
        /// <param name="handlerFactory"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="mapper"></param>
        /// <param name="permanentHandlersConfig"></param>
        public HandlersService(
            ILogger<HandlersService> logger,
            IHandlerRepository handlerRepository,
            IHandlerFactory handlerFactory,
            IServiceProvider serviceProvider,
            IMapper mapper,
            PermanentHandlersConfig permanentHandlersConfig)
        {
            _logger = logger;
            _handlerRepository = handlerRepository;
            _serviceProvider = serviceProvider;
            _handlerFactory = handlerFactory;
            _permanentHandlersConfig = permanentHandlersConfig;
            _mapper = mapper;
            this.InitTaskAsynchronous = this.RegisterAndStartHandlersAsync();
        }

        #endregion Constructors

        #region Publics which implement IDisposable

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (IDynamicHandler handler in this.RegisteredHandlers.Values)
                    {
                        handler.Dispose();
                    }
                    this.RegisteredHandlers.Clear();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Publics which implement IDisposable

        #region Publics wich implement IHandlersService

        ///<inheritdoc/>
        public async Task<Handler> AddOrUpdateHandlerAsync(Handler request)
        {
            // Vérification par le settings en cache, et non pas par l'enregistrement des handlers.
            if (_permanentHandlersConfig != null && _permanentHandlersConfig.Handlers.Any(x => x.Id == request.Id))
            {
                _logger.LogError("Il n'est pas possible d'ajouter un handler déjà existant dans les handlers permanents pour le handler {handler}.", request.Id);
                throw new Exception("Il n'est pas possible d'ajouter un handler déjà existant dans les handlers permanents.");
            }
            try
            {
                if (this.RegisteredHandlers.TryGetValue(request.Id, out IDynamicHandler handler) && handler != null)
                {
                    // Handler existe déjà, suppression du handler et de ses bindings actuels
                    _ = await _handlerRepository.RemoveAsync(handler.Id);
                    if (this.RegisteredHandlers.ContainsKey(request.Id))
                    {
                        _ = this.RegisteredHandlers.Remove(request.Id);
                    }
                }
                // Création d'un nouveau handler avec les nouveaux bindings
                Handler handlerRequest = _mapper.Map<Handler>(request);
                // Handler n'existe pas, création d'un nouveau avec les bindings
                Handler newHandler = await _handlerRepository.UpsertAsync(handlerRequest);
                this.RegisterHandler(newHandler);
                return newHandler;
            }
            catch (Exception ex)
            {
                // Gestion de l'exception
                _logger.LogError(ex, "Erreur lors de la création ou de la mise à jour du gestionnaire.");
                throw;
            }
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> StartHandlerAsync(string key)
        {
            var ret = new ActionResultContract();
            if (this.InitTaskAsynchronous == null || !this.InitTaskAsynchronous.IsCompleted)
            {
                ret.IsError = true;
                return ret;
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                try
                {
                    if (this.RegisteredHandlers.TryGetValue(key, out IDynamicHandler handler) && handler != null)
                    {
                        await this.StartHandlerAsync(key, handler);
                        ret.AddHandler(key);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error during start of handler {key}", key);
                }
            }
            return ret;
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> StopHandlerAsync(string key)
        {
            var ret = new ActionResultContract();
            if (this.InitTaskAsynchronous == null || !this.InitTaskAsynchronous.IsCompleted)
            {
                ret.IsError = true;
                return ret;
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                try
                {
                    if (this.RegisteredHandlers.TryGetValue(key, out IDynamicHandler handler) && handler != null)
                    {
                        await this.StopHandlerAsync(key, handler);
                        ret.AddHandler(key);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error during stop of handler {key}", key);
                }
            }
            return ret;
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> StartOrStopHandlerAsync(string key)
        {
            var ret = new ActionResultContract();
            if (this.InitTaskAsynchronous == null || !this.InitTaskAsynchronous.IsCompleted)
            {
                ret.IsError = true;
                return ret;
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                try
                {
                    if (this.RegisteredHandlers.TryGetValue(key, out IDynamicHandler handler) && handler != null)
                    {
                        if (handler.IsActive)
                        {
                            await this.StopHandlerAsync(key, handler);
                        }
                        else
                        {
                            await this.StartHandlerAsync(key, handler);
                        }
                        ret.AddHandler(key);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error during start or stop of handler {key}", key);
                }
            }
            return ret;
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> RestartHanderAsync(string key)
        {
            var ret = new ActionResultContract();
            if (this.InitTaskAsynchronous == null || !this.InitTaskAsynchronous.IsCompleted)
            {
                ret.IsError = true;
                return ret;
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                try
                {
                    if (this.RegisteredHandlers.TryGetValue(key, out IDynamicHandler handler) && handler != null)
                    {
                        if (handler.IsActive)
                        {
                            await this.StopHandlerAsync(key, handler);
                            await Task.Delay(500);
                            ret.IsPartialSuccess = true;
                        }
                        await this.StartHandlerAsync(key, handler);
                        ret.AddHandler(key);
                        ret.IsPartialSuccess = false;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error during restart of handler {key}", key);
                }
            }
            return ret;
        }

        //<inheritdoc/>
        public Handler GetHandlerStatus(string key)
        {
            var ret = new Handler();

            if (this.RegisteredHandlers.TryGetValue(key, out IDynamicHandler handler) && handler != null)
            {
                ret = _mapper.Map<Handler>(handler);
            }
            return ret;
        }

        ///<inheritdoc/>
        public async Task<PaginatedResponse<Handler>> GetHandlerStatus(PagingFilter pagingFilter = null)
        {
            // Mapper les valeurs enregistrées en liste
            List<Handler> handlers = _mapper.Map<List<Handler>>(this.RegisteredHandlers.Values);
            int totalItems = handlers.Count;

            // Vérifier si la pagination est désactivée
            if (pagingFilter?.PageSize == -1 && pagingFilter?.Page == -1)
            {
                var response = new PaginatedResponse<Handler>
                {
                    Items = handlers,
                    TotalItems = totalItems
                };
                return await Task.FromResult(response);
            }

            // Appliquer la pagination
            int pageSize = pagingFilter?.PageSize ?? 15;
            int page = pagingFilter?.Page ?? 1;

            // Ajuster les paramètres de la pagination
            handlers = handlers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var paginatedResponse = new PaginatedResponse<Handler>
            {
                Items = handlers,
                TotalItems = totalItems
            };

            return await Task.FromResult(paginatedResponse);
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> StartOrStopAllHandlersAsync()
        {
            var ret = new ActionResultContract();
            if (this.InitTaskAsynchronous == null || !this.InitTaskAsynchronous.IsCompleted)
            {
                ret.IsError = true;
                return ret;
            }
            foreach (string key in this.RegisteredHandlers.Select(x => x.Key))
            {
                ActionResultContract result = await this.StartOrStopHandlerAsync(key);
                if (!result.IsError)
                {
                    ret.AddHandler(key);
                }
                else
                {
                    ret.IsPartialSuccess = true;
                }
            }
            return ret;
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> StartAllHandlersAsync()
        {
            var ret = new ActionResultContract();
            if (this.InitTaskAsynchronous == null || !this.InitTaskAsynchronous.IsCompleted)
            {
                ret.IsError = true;
                return ret;
            }
            foreach (string key in this.RegisteredHandlers.Select(x => x.Key))
            {
                ActionResultContract result = await this.StartHandlerAsync(key);
                if (!result.IsError)
                {
                    ret.AddHandler(key);
                }
                else
                {
                    ret.IsPartialSuccess = true;
                }
            }
            return ret;
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> StopAllHanldersAsync()
        {
            var ret = new ActionResultContract();
            if (this.InitTaskAsynchronous == null || !this.InitTaskAsynchronous.IsCompleted)
            {
                ret.IsError = true;
                return ret;
            }
            foreach (string key in this.RegisteredHandlers.Select(x => x.Key))
            {
                ActionResultContract result = await this.StopHandlerAsync(key);
                if (!result.IsError)
                {
                    ret.AddHandler(key);
                }
                else
                {
                    ret.IsPartialSuccess = true;
                }
            }
            return ret;
        }

        ///<inheritdoc/>
        public async Task<bool> ReloadHandlersAsync()
        {
            bool result = true;
            this.InitTaskAsynchronous = null;
            try
            {
                // Stop all without db registration.
                foreach (IDynamicHandler handler in this.RegisteredHandlers.Values)
                {
                    if (handler.IsActive)
                    {
                        await handler.StopAsync(new System.Threading.CancellationToken());
                    }
                    handler.Dispose();
                }
                // Clear Registration.
                this.RegisteredHandlers.Clear();
                // Restart connections.
                await _handlerFactory.RestartConnectionsOfHandlersAsync();
                // Register.
                this.InitTaskAsynchronous = this.RegisterAndStartHandlersAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during reloading of Handlers. App need to be restarted");
                result = false;
            }
            return result;
        }

        ///<inheritdoc/>
        public async Task<ActionResultContract> RemoveHandlerAsync(string key)
        {
            var result = new ActionResultContract();
            // Vérification par le settings en cache, et non pas par l'enregistrement des handlers.
            if (_permanentHandlersConfig != null && _permanentHandlersConfig.Handlers.Any(x => x.Id == key))
            {
                _logger.LogError("Il n'est pas possible de supprimer un handler déjà existant dans les handlers permanents pour le handler {handler}.", key);
                result.IsError = true;
            }
            else
            {
                result = await this.StopHandlerAsync(key);
            }
            if (!result.IsError && this.RegisteredHandlers.TryGetValue(key, out IDynamicHandler handler) && handler != null)
            {
                result.AddHandler(key);
                handler.Dispose();
                bool cacheRemovalResult = this.RegisteredHandlers.Remove(key);
                bool persistenceRemovalResult = await _handlerRepository.RemoveAsync(key);
                // If cache result = true and persistence result = true, there is no error => So IsError = false.
                result.IsError = !(cacheRemovalResult && persistenceRemovalResult);
            }
            return result;
        }

        #endregion

        #region Privates

        /// <summary>
        /// Start all registers handlers (in DB and settings).
        /// </summary>
        /// <returns>Task wich need to be check on loading</returns>
        public async Task RegisterAndStartHandlersAsync()
        {
            await this.RegisterAndActivateAllActiveStoredHandlersAsync();

            await this.RegisterAllInactiveStoredHandlersAsync();

            await this.ConfigureAndActivatePermanentHandlersAsync();
        }

        /// <summary>
        /// Basic start of an handler with DbRegistration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private async Task StartHandlerAsync(string key, IDynamicHandler handler)
        {
            await handler.StartAsync(new System.Threading.CancellationToken());
            _ = await _handlerRepository.UpdateAsync(key, true);
            _logger.LogInformation("Handler {key} is started", key);
        }

        /// <summary>
        /// Basic stop of an handler (with db registration).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private async Task StopHandlerAsync(string key, IDynamicHandler handler)
        {
            await handler.StopAsync(new System.Threading.CancellationToken());
            _ = await _handlerRepository.UpdateAsync(key, false);
            _logger.LogInformation("Handler {key} is stopped", key);
        }

        /// <summary>
        /// Register all handlers which are stored in DB and which are actives.
        /// </summary>
        private async Task RegisterAndActivateAllActiveStoredHandlersAsync()
        {
            //Register handlers.
            List<Handler> handlers = await _handlerRepository.GetAllActiveHandlersAsync();
            foreach (Handler handlerDef in handlers)
            {
                this.RegisterHandler(handlerDef);
            }
            // Start handlers.
            foreach (KeyValuePair<string, IDynamicHandler> registeredHandler in this.RegisteredHandlers)
            {
                await registeredHandler.Value.StartAsync(new System.Threading.CancellationToken());
            }
        }

        /// <summary>
        /// Register all handlers which are stored in DB and which are not actives.
        /// </summary>
        private async Task RegisterAllInactiveStoredHandlersAsync()
        {
            //Register handlers.
            List<Handler> handlers = await _handlerRepository.GetAllInactiveHandlersAsync();
            foreach (Handler handlerDef in handlers)
            {
                this.RegisterHandler(handlerDef);
            }

        }

        /// <summary>
        /// Configure handler with permannet settings, and activate permanent setting wich not already on RegistredHandlers.
        /// </summary>
        /// <returns></returns>
        private async Task ConfigureAndActivatePermanentHandlersAsync()
        {
            await this.AddAndActivatePermanentHandlerAsync();

            this.PropagatePermanentSettingOnRegistredHandlers();
        }

        /// <summary>
        /// Add permanent handler wich are not already registred and activate it by default.
        /// </summary>
        /// <returns></returns>
        private async Task AddAndActivatePermanentHandlerAsync()
        {
            // Récupération des handlers permanents qui n'étaient pas déjà enregistrés en base, et donc lancés automatiquement.
            foreach (Handler nonRegisteredHandler in _permanentHandlersConfig?.Handlers
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Id) && !this.RegisteredHandlers.ContainsKey(x.Id)))
            {
                var handler = new Handler
                {
                    AssociateCommand = nonRegisteredHandler.AssociateCommand,
                    IsActive = true,
                    Id = nonRegisteredHandler.Id,
                    IsPermanent = true,
                    MessageIsStored = nonRegisteredHandler.MessageIsStored,
                    Queue = nonRegisteredHandler.Queue,
                    TimeToLiveInDays = nonRegisteredHandler.TimeToLiveInDays
                };
                if (nonRegisteredHandler.Bindings.Any())
                {
                    handler.AddBindings(nonRegisteredHandler.Bindings.Select(
                        x => new Binding
                        {
                            HandlerId = nonRegisteredHandler.Id,
                            Exchange = x.Exchange,
                            RoutingKey = x.RoutingKey
                        }).ToList());
                }
                await this.AddPermanentHandlerAsync(handler);
                if (this.RegisteredHandlers.TryGetValue(handler.Id, out IDynamicHandler dynamicHandler))
                {
                    await dynamicHandler.StartAsync(new System.Threading.CancellationToken());
                }
            }
        }

        /// <summary>
        /// Set IsPermanent value intoHandlers to true for each handler on permanent handler setting.
        /// </summary>
        private void PropagatePermanentSettingOnRegistredHandlers()
        {
            // Ajout de la récupération dynamique de la permanence, car pas enregistrée en base.
            foreach (KeyValuePair<string, IDynamicHandler> dynamicHandler in
                this.RegisteredHandlers.Where(x => _permanentHandlersConfig.Handlers.Select(h => h.Id).Contains(x.Key)))
            {
                dynamicHandler.Value.IsPermanent = true;
            }
        }

        /// <summary>
        /// Add a permanent handler.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        private async Task AddPermanentHandlerAsync(Handler handler)
        {
            try
            {
                handler.IsPermanent = true;
                // Handler n'existe pas, création d'un nouveau avec les bindings
                _ = await _handlerRepository.UpsertAsync(handler);
                this.RegisterHandler(handler);
            }
            catch (Exception ex)
            {
                // Gestion de l'exception
                _logger.LogError(ex, "Erreur lors de la création ou de la mise à jour du handler {handler}.", handler?.Id);
                throw;
            }
        }

        /// <summary>
        /// Register a specific handler.
        /// </summary>
        /// <param name="handlerDefinition"></param>
        private void RegisterHandler(Handler handlerDefinition)
        {
            async Task executionMethod(string message)
            {
                using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
                IMediator mediator = scope.ServiceProvider.GetService<IMediator>();
                await mediator.Send(new ReceiveMessageCommand(new MessageReceivedData(message, handlerDefinition)));
            }
            ;
            IDynamicHandler handler = _handlerFactory.CreateHandler(handlerDefinition, executionMethod);
            this.RegisteredHandlers.Add(handlerDefinition.Id, handler);
        }

        #endregion Privates
    }
}
