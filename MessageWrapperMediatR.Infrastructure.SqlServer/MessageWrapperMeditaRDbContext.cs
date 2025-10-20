using MessageWrapperMediatR.Domain.Models;
using MessageWrapperMediatR.Infrastructure.SqlServer.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.SqlServer
{
    public class MessageWrapperMeditaRDbContext : DbContext
    {
        internal const string SchemaName = "MessageWrapperMeditaR";

        public DbSet<MessageCollector> MessagesCollector { get; set; }

        public DbSet<Handler> Handlers { get; set; }
        public DbSet<Binding> Bindings { get; set; }

        public MessageWrapperMeditaRDbContext(DbContextOptions<MessageWrapperMeditaRDbContext> options)
           : base(options)
        {
            //this.Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessageWrapperMeditaRDbContext).Assembly);
            _ = modelBuilder.HasDefaultSchema(SchemaName);
            base.OnModelCreating(modelBuilder);
        }
    }
}
