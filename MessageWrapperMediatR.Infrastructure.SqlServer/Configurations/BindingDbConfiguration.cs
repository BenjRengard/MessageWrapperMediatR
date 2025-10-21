using MessageWrapperMediatR.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.SqlServer.Configurations
{
    public class BindingDbConfiguration : IEntityTypeConfiguration<Binding>
    {
        public void Configure(EntityTypeBuilder<Binding> builder)
        {
            builder.ToTable("Bindings");
            builder.HasKey(e => new { e.Exchange, e.RoutingKey, e.HandlerId });
            builder.Property(e => e.Exchange).HasMaxLength(80).IsRequired();
            builder.Property(e => e.RoutingKey).HasMaxLength(80).IsRequired();
            builder.HasOne(e => e.Handler).WithMany(h => h.Bindings).HasForeignKey(e => e.HandlerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
