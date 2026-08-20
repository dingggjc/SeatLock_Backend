using Microsoft.EntityFrameworkCore;
using SeatLock.Application.Common.Interfaces;
using SeatLock.Persistence.Data;
using SeatLock.Persistence.Identity;
using Xunit;

namespace SeatLock.Tests;

public sealed class TenantQueryFilterTests
{
    [Fact]
    public async Task UserQueriesAreEmptyWhenTheTenantContextIsMissing()
    {
        var tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<SeatLockDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using (var setupContext = new SeatLockDbContext(options, new TestTenantContext(null)))
        {
            setupContext.Users.Add(CreateUser(tenantId, "member@example.com"));
            await setupContext.SaveChangesAsync();
        }

        await using var context = new SeatLockDbContext(options, new TestTenantContext(null));
        Assert.Empty(await context.Users.ToListAsync());
    }

    [Fact]
    public async Task UserQueriesReturnOnlyTheCurrentTenant()
    {
        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<SeatLockDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using (var setupContext = new SeatLockDbContext(options, new TestTenantContext(null)))
        {
            setupContext.Users.AddRange(
                CreateUser(tenantId, "one@example.com"),
                CreateUser(otherTenantId, "two@example.com"));
            await setupContext.SaveChangesAsync();
        }

        await using var context = new SeatLockDbContext(options, new TestTenantContext(tenantId));
        var users = await context.Users.ToListAsync();

        var user = Assert.Single(users);
        Assert.Equal(tenantId, user.TenantId);
    }

    private static ApplicationUser CreateUser(Guid tenantId, string email) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        UserName = email,
        NormalizedUserName = email.ToUpperInvariant(),
        Email = email,
        NormalizedEmail = email.ToUpperInvariant()
    };

    private sealed class TestTenantContext(Guid? tenantId) : ITenantContext
    {
        public Guid? TenantId { get; } = tenantId;
    }
}
