using MessageWrapperMediatR.Core.Filters;
using MessageWrapperMediatR.Core.Models;

namespace MessageWrapperMediatR.Core.Repositories
{
    /// <summary>
    /// Collected message repository
    /// </summary>
    public interface ICollectedMessageRepository
    {
        /// <summary>
        /// Add a message into Collected Messages.
        /// </summary>
        /// <param name="messageContent">Message content.</param>
        /// <param name="handlerFrom">Handler from the message.</param>
        /// <returns>The new Collected message.</returns>
        Task<CollectedMessage> AddCollectedMessageAsync(string messageContent, Handler handlerFrom);

        /// <summary>
        /// Update the status of a collected message.
        /// </summary>
        /// <param name="messagesIdsToUpdate">Ids of messages to update.</param>
        /// <param name="status">Status to set. By default is true.</param>
        /// <returns>Void.</returns>
        Task UpdateStatusOfCollectedMessagesAsync(IEnumerable<Guid> messagesIdsToUpdate, bool status = true);

        /// <summary>
        /// Get the list of message to purge by the rule of TTL and the date to calculate.
        /// </summary>
        /// <param name="now">Date to calculate the list.</param>
        /// <returns>List of collected messages to purge.</returns>
        Task<List<CollectedMessage>> GetMessagesToPurgeAsync(DateTimeOffset now);

        /// <summary>
        /// Delete a list of collected messages.
        /// </summary>
        /// <param name="messages">Collected messages to delete.</param>
        /// <returns>Void.</returns>
        Task DeleteMessagesAsync(List<CollectedMessage> messages);

        /// <summary>
        /// Delete one message by ID.
        /// </summary>
        /// <param name="messageId">Collected message Id.</param>
        /// <returns>Ture if success. False if it's not.</returns>
        Task<bool> DeleteMessageByIdAsync(Guid messageId);

        /// <summary>
        /// Get messages by filters.
        /// </summary>
        /// <param name="queues">List of queues.</param>
        /// <param name="receptionBegin">Reception date begin.</param>
        /// <param name="receptionEnd">Reception date end.</param>
        /// <param name="ids">List of Ids.</param>
        /// <param name="associateCommands">List of associate commands.</param>
        /// <param name="isHandled">If messages are handled.</param>
        /// <param name="pagingFilter">Filter of pagination.</param>
        /// <returns>Paginated list of Collected messages.</returns>
        Task<PaginatedResponse<CollectedMessage>> GetCollectedMessagesAsync(
                                                  List<string> queues,
                                                  DateTimeOffset? receptionBegin,
                                                  DateTimeOffset? receptionEnd,
                                                  List<Guid> ids,
                                                  List<string> associateCommands,
                                                  bool? isHandled = null,
                                                  PagingFilter? pagingFilter = null);

        /// <summary>
        /// Get a collected message by id.
        /// </summary>
        /// <param name="messageId">Id of collected message.</param>
        /// <returns>Collected message.</returns>
        Task<CollectedMessage> GetCollectedMessageByIdAsync(Guid messageId);
    }
}
