using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatLock.Domain.Entities;

namespace SeatLock.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(value => value.Id);
        builder.HasIndex(value => value.Slug).IsUnique();
        builder.Property(value => value.Slug).HasMaxLength(100).IsRequired();
        builder.Property(value => value.Name).HasMaxLength(200).IsRequired();
    }
}
