using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeatLock.Application.Authentication;
using SeatLock.Application.Authentication.DTO;
using SeatLock.Domain.Entities;
using SeatLock.Persistence.Data;
using SeatLock.Persistence.Identity;

namespace SeatLock.Persistence.Authentication;

public sealed class IdentityAuthenticationService(
    SeatLockDbContext context,
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    TimeProvider timeProvider) : IAuthenticationService
{
    public async Task<AuthTokenResultDTO?> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken)
    {
        var normalizedEmail = userManager.NormalizeEmail(request.Email);
        var user = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(
            value => value.NormalizedEmail == normalizedEmail,
            cancellationToken);
        if (user is null || await userManager.IsLockedOutAsync(user)) return null;

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);

        return await IssueTokensAsync(user, user.TenantId, cancellationToken);
    }

    public async Task<AuthTokenResultDTO?> RefreshAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var hash = tokenService.HashRefreshToken(request.RefreshToken);
        var refreshToken = await context.RefreshTokens.IgnoreQueryFilters().SingleOrDefaultAsync(value => value.TokenHash == hash, cancellationToken);
        if (refreshToken is null || !refreshToken.IsActive(now)) return null;

        var user = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(value => value.Id == refreshToken.UserId, cancellationToken);
        if (user is null || user.TenantId != refreshToken.TenantId) return null;

        refreshToken.RevokedAtUtc = now;
        var response = await IssueTokensAsync(user, refreshToken.TenantId, saveChanges: false, cancellationToken: cancellationToken);
        refreshToken.ReplacedByTokenHash = tokenService.HashRefreshToken(response.RefreshToken);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return response;
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
    }

    private async Task<AuthTokenResultDTO> IssueTokensAsync(ApplicationUser user, Guid tenantId, CancellationToken cancellationToken) =>
        await IssueTokensAsync(user, tenantId, saveChanges: true, cancellationToken);

    private async Task<AuthTokenResultDTO> IssueTokensAsync(ApplicationUser user, Guid tenantId, bool saveChanges, CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var access = tokenService.CreateAccessToken(user.Id, tenantId, user.Email!, roles.ToArray());
        var refreshToken = tokenService.CreateRefreshToken();
        var now = timeProvider.GetUtcNow();
        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TenantId = tenantId,
            TokenHash = tokenService.HashRefreshToken(refreshToken),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(tokenService.RefreshTokenDays)
        });
        if (saveChanges)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return new AuthTokenResultDTO
        {
            AccessToken = access.Token,
            AccessTokenExpiresAtUtc = access.ExpiresAtUtc,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = now.AddDays(tokenService.RefreshTokenDays)
        };
    }
}
