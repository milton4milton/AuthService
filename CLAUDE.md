# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# First-time setup — set secrets before running
dotnet user-secrets set "JwtSettings:SecretKey" "<your-32+-char-key>" --project AuthService.API
dotnet user-secrets set "SeedData:AdminPassword" "SuperAdmin@123" --project AuthService.API

# Run the API (Swagger opens at http://localhost:5222/swagger)
dotnet run --project AuthService.API

# Build entire solution
dotnet build AdminMicroservices.sln

# Add a new EF Core migration
dotnet ef migrations add <MigrationName> --project AuthService.Infrastructure --startup-project AuthService.API

# Apply migrations to the database
dotnet ef database update --project AuthService.Infrastructure --startup-project AuthService.API

# Restore packages
dotnet restore
```

No test projects exist yet. When adding one, use `dotnet test` from the solution root.

## Architecture

Clean Architecture with four projects. Dependencies only flow inward:

```
AuthService.API  →  AuthService.Application  →  AuthService.Domain
                         ↑
              AuthService.Infrastructure ─────────────────────────→ AuthService.Domain
```

- **Domain** — Entities, enums, value objects, repository interfaces. Zero external dependencies.
- **Application** — Service interfaces (`IUserService`, `IRoleService`, `IMenuService`), DTOs, service implementations, and `DependencyInjection.cs` (`AddApplication()`).
- **Infrastructure** — EF Core `AuthDbContext`, repository implementations, migrations, `UnitOfWork`, `DbInitializer`, and `DependencyInjection.cs` (`AddInfrastructure()`).
- **API** — Controllers, `ExceptionMiddleware`, `JwtSettings` helper. `Program.cs` wires everything via `AddInfrastructure()` and `AddApplication()`.

## Key Patterns

**Repository + Unit of Work** — All data access goes through `IUserRepository`, `IRoleRepository`, `IPermissionRepository`, `IMenuRepository`. Changes are committed via `IUnitOfWork.SaveChangesAsync()`. Never call `DbContext.SaveChangesAsync()` directly from services.

**JWT flow** — `POST /api/auth/login` validates credentials via `IUserService.ValidateUserAsync` (BCrypt), then `AuthController.GenerateJwtToken` builds a token with `sub`, `email`, and one `ClaimTypes.Role` claim per assigned role. The token is validated on every protected endpoint via the `[Authorize]` attribute.

**DB Seeding** — `DbInitializer.SeedAsync` runs at startup and creates a "Super Admin" role and `superadmin@admin.com` user only if no roles exist yet. It is idempotent.

**EF Configuration** — All table mappings use Fluent API files in `AuthService.Infrastructure/Configurations/`. Entities have no data-annotation attributes.

**No CQRS** — `Commands/` and `Queries/` folders were removed; MediatR is not installed. Services call repositories directly. If you want CQRS, install MediatR first.

## Known Issues / Gotchas

- **Secrets are required at startup** — the app throws `InvalidOperationException` if `JwtSettings:SecretKey` or `SeedData:AdminPassword` are not set. Use `dotnet user-secrets` locally; env vars in production (`JwtSettings__SecretKey`, `SeedData__AdminPassword`).
- `AuthController.cs` contains a commented-out duplicate `Login` method at the top — ignore it.
- `MenuService` is fully implemented but no `MenuController` exposes it yet.

## CORS

Configured for `http://localhost:5173` (Vite/React frontend). Add origins in `Program.cs` under the `"AllowReactApp"` policy when deploying to other environments.

## Default Credentials (Development Only)

| Field    | Value                  |
|----------|------------------------|
| Email    | `superadmin@admin.com` |
| Password | `SuperAdmin@123`       |
| Role     | Super Admin            |
