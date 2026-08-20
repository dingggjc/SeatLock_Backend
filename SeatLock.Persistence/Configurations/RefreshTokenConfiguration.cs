using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatLock.Domain.Entities;

namespace SeatLock.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(value => value.Id);
        builder.HasIndex(value => value.TokenHash).IsUnique();
        builder.HasIndex(value => new { value.TenantId, value.UserId });
        builder.Property(value => value.TokenHash).HasMaxLength(128).IsRequired();
        builder.Property(value => value.RowVersion).IsRowVersion();
    }
}
