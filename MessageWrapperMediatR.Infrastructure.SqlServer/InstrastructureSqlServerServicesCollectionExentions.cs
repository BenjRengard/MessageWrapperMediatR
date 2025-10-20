using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
