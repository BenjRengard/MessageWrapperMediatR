using MessageWrapperMediatR.Core.Contracts;
using MessageWrapperMediatR.Core.Filters;
using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Application.Services
{
    public interface IHandlersService
    {
        /// <summary>
        /// Stop the handler corresponding with the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if no error.</returns>
        Task<ActionResultContract> StopHandlerAsync(string key);

        /// <summary>
        /// Start the handler corresponding wtich the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Tru if no error.</returns>
        Task<ActionResultContract> StartHandlerAsync(string key);

        /// <summary>
        /// Start or stop the handler.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ActionResultContract> StartOrStopHandlerAsync(string key);

        /// <summary>
        /// Get handler status.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Handler GetHandlerStatus(string key);

        /// <summary>
        /// Get all handers status.
        /// </summary>
        /// <returns></returns>
        Task<PaginatedResponse<Handler>> GetHandlerStatus(PagingFilter pagingFilter = null);

        /// <summary>
        /// Restart handler.
        /// If handler is Stopped, handler is started.
        /// If hanlder is Started, handler is stopped and start.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ActionResultContract> RestartHanderAsync(string key);

        /// <summary>
        /// Start or stop all the registred handlers.
        /// </summary>
        /// <returns></returns>
        Task<ActionResultContract> StartOrStopAllHandlersAsync();

        /// <summary>
        /// Start all the registred handlers.
        /// </summary>
        /// <returns></returns>
        Task<ActionResultContract> StartAllHandlersAsync();

        /// <summary>
        /// Stop all the registred handlers.
        /// </summary>
        /// <returns></returns>
        Task<ActionResultContract> StopAllHanldersAsync();

        /// <summary>
        /// Reload handlers and restore previous status for each of them.
        /// </summary>
        /// <returns></returns>
        Task<bool> ReloadHandlersAsync();

        /// <summary>
        /// Add a handler in hot load, and register it.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Handler> AddOrUpdateHandlerAsync(Handler request);

        /// <summary>
        /// Remove handler in registration. Close it, suppress it on the registration, and in storage.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ActionResultContract> RemoveHandlerAsync(string key);
    }
}
