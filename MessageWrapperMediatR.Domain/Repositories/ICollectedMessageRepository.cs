using MessageWrapperMediatR.Domain.Filters;
using MessageWrapperMediatR.Domain.Models;

namespace MessageWrapperMediatR.Domain.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICollectedMessageRepository
    {
        Task<CollectedMessage> AddCollectedMessageAsync(string messageContent, Handler handlerFrom);

        Task UpdateStatusOfCollectedMessagesAsync(IEnumerable<Guid> messagesIdsToUpdate, bool status = true);

        Task<List<CollectedMessage>> GetMessagesToPurgeAsync(DateTimeOffset now);

        Task DeleteMessagesAsync(List<CollectedMessage> messages);

        Task<bool> DeleteMessageByIdAsync(Guid messageId);

        Task<PaginatedResponse<CollectedMessage>> GetCollectedMessagesAsync(
                                                  List<string> queues,
                                                  DateTimeOffset? receptionBegin,
                                                  DateTimeOffset? receptionEnd,
                                                  List<Guid> ids,
                                                  List<string> associateCommands,
                                                  bool? isHandled = null,
                                                  PagingFilter? pagingFilter = null);
        Task<CollectedMessage> GetCollectedMessageByIdAsync(Guid messageId);
    }
}
