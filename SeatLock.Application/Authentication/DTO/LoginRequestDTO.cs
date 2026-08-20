namespace SeatLock.Application.Authentication.DTO;

public sealed record LoginRequestDTO
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
