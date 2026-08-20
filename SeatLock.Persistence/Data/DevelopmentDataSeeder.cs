using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeatLock.Application.Authentication;
using SeatLock.Domain.Entities;
using SeatLock.Persistence.Identity;

namespace SeatLock.Persistence.Data;

public sealed class DevelopmentDataSeeder(
    SeatLockDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    TimeProvider timeProvider)
{
    public const string TenantSlug = "demo";
    public const string AdminEmail = "demo@seatlock.local";
    public const string AdminPassword = "DemoPassword123!";
    public const string CustomerEmail = "customer@seatlock.local";
    public const string CustomerPassword = "CustomerPassword123!";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var roleName in SeatLockRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create the {roleName} role: {string.Join("; ", roleResult.Errors.Select(error => error.Description))}");
                }
            }
        }

        var tenant = await context.Tenants.SingleOrDefaultAsync(
            value => value.Slug == TenantSlug,
            cancellationToken);

        if (tenant is null)
        {
            tenant = new Tenant
            {
                Slug = TenantSlug,
                Name = "SeatLock Demo",
                CreatedAtUtc = timeProvider.GetUtcNow()
            };
            context.Tenants.Add(tenant);
            await context.SaveChangesAsync(cancellationToken);
        }

        await SeedUserAsync(tenant, AdminEmail, AdminPassword, SeatLockRoles.Admin, cancellationToken);
        await SeedUserAsync(tenant, CustomerEmail, CustomerPassword, SeatLockRoles.Customer, cancellationToken);
    }

    private async Task SeedUserAsync(Tenant tenant, string email, string password, string role, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(email);
        var user = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(
            value => value.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (user is null)
        {
            user = new ApplicationUser
            {
                TenantId = tenant.Id,
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unable to create the {role} demo user: {string.Join("; ", result.Errors.Select(error => error.Description))}");
            }
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, resetToken, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unable to reset the {role} demo user's password: {string.Join("; ", result.Errors.Select(error => error.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to assign the demo user to the {role} role: {string.Join("; ", roleResult.Errors.Select(error => error.Description))}");
            }
        }
    }
}
