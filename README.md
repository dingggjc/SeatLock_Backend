# SeatLock API

SeatLock is a .NET 10 multi-tenant API using ASP.NET Core Identity and JWT bearer authentication.

## Authentication

Sign in with a globally unique email address and password. SeatLock resolves the
user's tenant automatically and includes that tenant ID in the access token, so
the login request does not include a tenant slug.

`GET /api/auth/me` requires an `Authorization: Bearer <access-token>` header
and returns the authenticated user's ID, email, tenant ID, and roles. It never
accepts a user ID from the client.

The available roles are `Admin` and `Customer`. Development seeding creates an
Admin account (`demo@seatlock.local`) and Customer account (`customer@seatlock.local`).

## Local configuration

Copy `SeatLock.WebApi/appsettings.Development.template.json` to
`appsettings.Development.json`, then put the local SQL Server connection string
and JWT signing key in that file. It is ignored by Git.

In Development, API startup automatically applies pending EF Core migrations and
seeds the Admin and Customer demo accounts.

## Verify

```powershell
dotnet restore SeatLock.slnx
dotnet build SeatLock.slnx --no-restore
dotnet test SeatLock.slnx --no-build
```

## EF Core migrations

Migrations are automatic during Development startup. For manual commands, EF
Core reads `ConnectionStrings:DefaultConnection` directly from
`SeatLock.WebApi/appsettings.Development.json`.

```powershell
dotnet ef migrations add <MigrationName> --project SeatLock.Persistence --startup-project SeatLock.Persistence
dotnet ef database update --project SeatLock.Persistence --startup-project SeatLock.Persistence
```

In Visual Studio's Package Manager Console, use:

```powershell
Update-Database -Project SeatLock.Persistence -StartupProject SeatLock.WebApi
```
