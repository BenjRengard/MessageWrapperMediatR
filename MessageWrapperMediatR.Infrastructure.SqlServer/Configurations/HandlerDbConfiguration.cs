using MessageWrapperMediatR.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageWrapperMediatR.Infrastructure.SqlServer.Configurations
{
    public class HandlerDbConfiguration : IEntityTypeConfiguration<Handler>
    {
        public void Configure(EntityTypeBuilder<Handler> builder)
        {
            _ = builder.ToTable("Handlers");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Queue).HasMaxLength(80).IsRequired(true);
            _ = builder.Property(e => e.AssociateCommand).HasMaxLength(80);
            _ = builder.Property(e => e.Id).HasMaxLength(100);
            _ = builder.Property(e => e.TimeToLiveInDays).HasDefaultValue(7).IsRequired();
            _ = builder.Property(e => e.IsStored).HasDefaultValue(true);
            _ = builder.Ignore(e => e.IsPermanent);
            _ = builder.HasMany(h => h.Bindings).WithOne(b => b.Handler).HasForeignKey(b => b.HandlerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
