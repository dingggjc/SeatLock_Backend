using System.Security.Claims;
using SeatLock.Application.Common.Interfaces;

namespace SeatLock.WebApi.Services;

public sealed class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    public Guid? TenantId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue("tenant_id"), out var id) ? id : null;
}
