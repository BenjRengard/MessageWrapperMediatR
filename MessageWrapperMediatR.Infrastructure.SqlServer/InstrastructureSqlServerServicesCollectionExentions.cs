using MessageWrapperMediatR.Core.Repositories;
using MessageWrapperMediatR.Infrastructure.SqlServer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageWrapperMediatR.Infrastructure.SqlServer
{
    public static class InstrastructureSqlServerServicesCollectionExentions
    {
        public static DbContextOptions<MessageWrapperMeditaRDbContext> GetSqlServerMessageWrapperOptions(this IConfiguration configuration)
        {
            return new DbContextOptionsBuilder<MessageWrapperMeditaRDbContext>()
             .UseSqlServer(configuration.GetConnectionString("MessageWrapperDatabaseConnection"))
             .Options;
        }

        public static IServiceCollection AddSqlServerMessageWrapper(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddDbContext<MessageWrapperMeditaRDbContext>(options =>
                         options.UseSqlServer(configuration.GetConnectionString("MessageWrapperDatabaseConnection"),
                         x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, MessageWrapperMeditaRDbContext.SchemaName)));

            _ = services.AddSingleton<IHandlerRepository, HandlerRepository>();
            _ = services.AddScoped<ICollectedMessageRepository, CollectedMessageRepository>();

            return services;
        }
    }
}
