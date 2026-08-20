namespace SeatLock.Application.Authentication.DTO;

public sealed record CurrentUserProfileDTO
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required Guid TenantId { get; init; }
    public required IReadOnlyCollection<string> Roles { get; init; }
}
