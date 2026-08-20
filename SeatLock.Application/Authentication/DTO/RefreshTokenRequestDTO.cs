namespace SeatLock.Application.Authentication.DTO;

public sealed record RefreshTokenRequestDTO
{
    public required string RefreshToken { get; init; }
}
