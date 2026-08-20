namespace SeatLock.Application.Authentication;

public interface ITokenService
{
    int RefreshTokenDays { get; }
    (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(Guid userId, Guid tenantId, string email, IReadOnlyCollection<string> roles);
    string CreateRefreshToken();
    string HashRefreshToken(string token);
}
