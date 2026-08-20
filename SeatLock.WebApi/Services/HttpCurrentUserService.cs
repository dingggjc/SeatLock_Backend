using System.Security.Claims;
using SeatLock.Application.Common.Interfaces;

namespace SeatLock.WebApi.Services;

public sealed class HttpCurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal User => accessor.HttpContext?.User ?? new ClaimsPrincipal();
    public Guid? UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id) ? id : null;
    public string? Email => User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
    public IReadOnlyCollection<string> Roles => User.FindAll(ClaimTypes.Role).Select(value => value.Value).ToArray();
}
