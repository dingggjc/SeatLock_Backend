using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SeatLock.Domain.Entities;
using SeatLock.Application.Common.Interfaces;
using SeatLock.Persistence.Identity;

namespace SeatLock.Persistence.Data;

public sealed class SeatLockDbContext(DbContextOptions<SeatLockDbContext> options, ITenantContext tenantContext)
    : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(SeatLockDbContext).Assembly);
        builder.Entity<ApplicationUser>().HasQueryFilter(value => tenantContext.TenantId != null && value.TenantId == tenantContext.TenantId);
        builder.Entity<RefreshToken>().HasQueryFilter(value => tenantContext.TenantId != null && value.TenantId == tenantContext.TenantId);
    }
}
