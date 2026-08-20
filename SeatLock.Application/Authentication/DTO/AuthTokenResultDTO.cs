namespace SeatLock.Application.Authentication.DTO;

public sealed record AuthTokenResultDTO
{
    public required string AccessToken { get; init; }
    public required DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }
}
