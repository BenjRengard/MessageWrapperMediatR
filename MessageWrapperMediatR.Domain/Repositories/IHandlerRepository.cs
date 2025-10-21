using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Core.Repositories
{
    /// <summary>
    /// Repository of handlers.
    /// </summary>
    public interface IHandlerRepository
    {
        /// <summary>
        /// Get a stored handler by id.
        /// </summary>
        /// <param name="id">Id of the handler.</param>
        /// <returns>The handler.</returns>
        Task<Handler?> GetByIdAsync(string id);

        /// <summary>
        /// Update a handler.
        /// </summary>
        /// <param name="id">Id of the handler to upsert.</param>
        /// <param name="isActive">Value of IsActive to insert.</param>
        /// <returns>The handler updated or created.</returns>
        Task<Handler?> UpdateAsync(string id, bool isActive);

        /// <summary>
        /// Update or insert a handler.
        /// </summary>
        /// <param name="handlerRequest">Handler to create or to update.</param>
        /// <returns>The handler updated or created.</returns>
        Task<Handler> UpsertAsync(Handler handlerRequest);

        /// <summary>
        /// Get all the handlers stored.
        /// </summary>
        /// <returns>List of all stored handlers.</returns>
        Task<List<Handler>> GetAllAsync();

        /// <summary>
        /// Get all actives stored handlers.
        /// </summary>
        /// <returns>List of all active stored handlers.</returns>
        Task<List<Handler>> GetAllActiveHandlersAsync();

        /// <summary>
        /// Get all inactive stored handlers.
        /// </summary>
        /// <returns>List of all inactive stored handlers.</returns>
        Task<List<Handler>> GetAllInactiveHandlersAsync();

        /// <summary>
        /// Remove a handler in store.
        /// </summary>
        /// <param name="id">Id of the handler to remove.</param>
        /// <returns>True if is success. False if it's not.</returns>
        Task<bool> RemoveAsync(string id);
    }
}
