using MessageWrapperMediatR.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageWrapperMediatR.Infrastructure.SqlServer
{
    public class MessageWrapperMeditaRDbContext : DbContext
    {
        internal const string SchemaName = "MessageWrapperMeditaR";

        public DbSet<CollectedMessage> CollectedMessages { get; set; }

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
