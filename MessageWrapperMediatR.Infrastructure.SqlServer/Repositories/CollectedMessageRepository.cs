using MessageWrapperMediatR.Core.Filters;
using MessageWrapperMediatR.Core.Models;
using MessageWrapperMediatR.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MessageWrapperMediatR.Infrastructure.SqlServer.Repositories
{
    ///<inheritdoc/>
    public class CollectedMessageRepository : ICollectedMessageRepository
    {
        private readonly DbContextOptions<MessageWrapperMeditaRDbContext> _options;

        public CollectedMessageRepository(IConfiguration configuration)
        {
            _options = configuration.GetSqlServerMessageWrapperOptions();
        }

        ///<inheritdoc/>
        public async Task<CollectedMessage> AddCollectedMessageAsync(string messageContent, Handler handlerFrom)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            var message = new CollectedMessage(messageContent, handlerFrom);
            _ = dbContext.CollectedMessages.Add(message);
            _ = await dbContext.SaveChangesAsync();
            return message;
        }

        ///<inheritdoc/>
        public async Task<bool> DeleteMessageByIdAsync(Guid messageId)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            CollectedMessage? message = await dbContext.CollectedMessages.FirstOrDefaultAsync(x => x.Id == messageId);
            if (message != null)
            {
                _ = dbContext.CollectedMessages.Remove(message);
                _ = await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        ///<inheritdoc/>
        public async Task DeleteMessagesAsync(List<CollectedMessage> messages)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            dbContext.CollectedMessages.RemoveRange(messages);
            await dbContext.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task<CollectedMessage> GetCollectedMessageByIdAsync(Guid messageId)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            return await dbContext.CollectedMessages.AsNoTracking().FirstOrDefaultAsync(c => c.Id == messageId);
        }

        ///<inheritdoc/>
        public async Task<PaginatedResponse<CollectedMessage>> GetCollectedMessagesAsync(
            List<string> queues,
            DateTimeOffset? receptionBegin,
            DateTimeOffset? receptionEnd,
            List<Guid> ids,
            List<string> associateCommands,
            bool? isHandled = null,
            PagingFilter? pagingFilter = null)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            IQueryable<CollectedMessage> query = dbContext.CollectedMessages.AsNoTracking()
                   .Where(x =>
                     (!queues.Any() || queues.Contains(x.FromQueue)) &&
                     (receptionBegin == null || x.ReceptionDate >= receptionBegin) &&
                     (receptionEnd == null || x.ReceptionDate <= receptionEnd) &&
                     (!ids.Any() || ids.Contains(x.Id)) &&
                     (!associateCommands.Any() || associateCommands.Contains(x.AssociateCommand)) &&
                     (isHandled == null || x.IsHandled == isHandled))
                   .OrderBy(x => x.ReceptionDate);

            // Get total items count
            int totalItems = await query.CountAsync();

            // Apply paging only if PageSize and Page are not -1
            if (pagingFilter != null && (pagingFilter.PageSize != -1 && pagingFilter.Page != -1))
            {
                int pageSize = pagingFilter.PageSize;
                int page = pagingFilter.Page;
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            List<CollectedMessage> messages = await query.ToListAsync();

            return new PaginatedResponse<CollectedMessage>
            {
                Items = messages,
                TotalItems = totalItems
            };
        }

        ///<inheritdoc/>
        public async Task<List<CollectedMessage>> GetMessagesToPurgeAsync(DateTimeOffset now)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            return await dbContext.CollectedMessages
                    .AsNoTracking()
                    .Where(m => m.IsHandled && m.ReceptionDate < DateTimeOffset.Now.AddDays(-m.TimeToLiveInDays))
                    .ToListAsync();
        }

        ///<inheritdoc/>
        public async Task UpdateStatusOfCollectedMessagesAsync(IEnumerable<Guid> messagesIdsToUpdate, bool status = true)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            foreach (Guid id in messagesIdsToUpdate)
            {
                CollectedMessage message = await dbContext.CollectedMessages.FirstOrDefaultAsync(x => x.Id == id);
                if (message != null)
                {
                    message.IsHandled = status;
                }
            }
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
