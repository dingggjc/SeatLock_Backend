namespace SeatLock.Application.Authentication;

public static class SeatLockRoles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";

    public static readonly IReadOnlyCollection<string> All = [Admin, Customer];
}
