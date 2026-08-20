using SeatLock.Application.Authentication.DTO;

namespace SeatLock.Application.Authentication;

public interface IAuthenticationService
{
    Task<AuthTokenResultDTO?> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken);
    Task<AuthTokenResultDTO?> RefreshAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken);
}
