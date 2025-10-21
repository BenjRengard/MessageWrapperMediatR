using MessageWrapperMediatR.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageWrapperMediatR.Infrastructure.SqlServer.Configurations
{
    public class CollectedMessageDbConfiguration : IEntityTypeConfiguration<CollectedMessage>
    {
        public void Configure(EntityTypeBuilder<CollectedMessage> builder)
        {
            _ = builder.ToTable("CollectedMessages");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.FromQueue).HasMaxLength(80).IsRequired(true);
            _ = builder.HasIndex(e => e.FromQueue).IsUnique(false);
            _ = builder.HasIndex(e => e.ReceptionDate).IsUnique(false);
            _ = builder.Property(e => e.TimeToLiveInDays).HasDefaultValue(7).IsRequired();
            _ = builder.Property(e => e.AssociateCommand).HasMaxLength(80);
        }
    }
}
