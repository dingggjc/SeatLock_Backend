using Microsoft.AspNetCore.Identity;

namespace SeatLock.Persistence.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; set; }
}
