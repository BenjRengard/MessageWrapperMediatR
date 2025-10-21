using MessageWrapperMediatR.Core.Models;
using MessageWrapperMediatR.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MessageWrapperMediatR.Infrastructure.SqlServer.Repositories
{
    public class HandlerRepository : IHandlerRepository
    {
        private readonly DbContextOptions<MessageWrapperMeditaRDbContext> _options;


        public HandlerRepository(IConfiguration configuration)
        {
            _options = configuration.GetSqlServerMessageWrapperOptions();
        }

        public async Task<List<Handler>> GetAllActiveHandlersAsync()
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            return await dbContext.Handlers.Include(h => h.Bindings).Where(c => c.IsActive == true).AsNoTracking().ToListAsync();
        }

        public async Task<List<Handler>> GetAllAsync()
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            return await dbContext.Handlers.Include(h => h.Bindings).AsNoTracking().ToListAsync();
        }

        public async Task<List<Handler>> GetAllInactiveHandlersAsync()
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            return await dbContext.Handlers.Include(h => h.Bindings).AsNoTracking().Where(c => c.IsActive == false).ToListAsync();
        }

        public async Task<Handler?> GetByIdAsync(string id)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            return await dbContext.Handlers.Include(h => h.Bindings).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> RemoveAsync(string id)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            bool result = false;
            Handler? handler = await dbContext.Handlers.FirstOrDefaultAsync(x => x.Id == id);
            if (handler != null)
            {
                _ = dbContext.Handlers.Remove(handler);
                int saveResult = await dbContext.SaveChangesAsync();
                result = saveResult > 0;
            }
            return result;
        }

        public async Task<Handler?> UpdateAsync(string id, bool isActive)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            Handler? handler = await dbContext.Handlers.FirstOrDefaultAsync(x => x.Id == id);
            if (handler != null)
            {
                handler.IsActive = isActive;
                _ = await dbContext.SaveChangesAsync();
            }
            return handler;
        }

        public async Task<Handler> UpsertAsync(Handler handlerRequest)
        {
            using var dbContext = new MessageWrapperMeditaRDbContext(_options);
            Handler? handler = await dbContext.Handlers.Include(h => h.Bindings).FirstOrDefaultAsync(x => x.Id == handlerRequest.Id);
            if (handler != null)
            {
                handler.ModifyHandler(handlerRequest);
            }
            else
            {
                //handler = new Handler(handlerRequest);
                _ = await dbContext.Handlers.AddAsync(handlerRequest);
            }
            _ = await dbContext.SaveChangesAsync();
            return handler;
        }
    }
}
