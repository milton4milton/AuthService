# AdminMicroservices — Architecture Guide & Improvement Roadmap

> **Stack:** .NET 9 · ASP.NET Core · EF Core 9 · SQL Server · JWT · BCrypt · Swagger  
> **Pattern:** Clean Architecture (Domain → Application → Infrastructure → API)

---

## 1. Current Architecture

```
AdminMicroservices.sln
├── AuthService.API/            ← Presentation: Controllers, Middleware, Program.cs
├── AuthService.Application/    ← Business Logic: Services, DTOs, Interfaces, Commands, Queries
├── AuthService.Domain/         ← Core: Entities, Enums, Interfaces, Value Objects
└── AuthService.Infrastructure/ ← Data: EF Core DbContext, Repositories, Migrations, UoW
```

### Dependency Flow (correct direction)
```
API → Application → Domain ← Infrastructure
```

### Key Components

| Layer          | Responsibility                                      | Files                                              |
|----------------|-----------------------------------------------------|----------------------------------------------------|
| Domain         | Entities, business rules, repository interfaces     | User, Role, Menu, Permission, Module, ValueObjects |
| Application    | Use cases, DTOs, service interfaces                 | UserService, RoleService, MenuService              |
| Infrastructure | EF Core, repositories, migrations, seeding          | AuthDbContext, Repositories, UnitOfWork            |
| API            | HTTP endpoints, JWT auth, middleware, DI setup      | AuthController, UserController, RoleController     |

### Authentication Flow
```
POST /api/auth/login
  → UserService.ValidateUserAsync()
  → BCrypt.Verify(password, hash)
  → JWT generated (sub, email, role claims)
  → AuthResponseDto returned
  → Client sends Bearer token on every protected request
```

---

## 2. What's Working Well

- **Clean Architecture** — proper layer separation; domain has zero external dependencies.
- **Repository + Unit of Work** — data access is abstracted; swappable without touching business logic.
- **EF Fluent Configuration** — all table mappings in `Configurations/`, not polluting entities with attributes.
- **BCrypt password hashing** — industry-standard; work factor prevents brute force.
- **Exception Middleware** — prevents stack traces leaking to clients.
- **DB Seeding** — idempotent; only seeds if roles don't exist.
- **Value Object for Email** — `Email.cs` enforces format at the domain level.

---

## 3. Critical Issues (Fix First)

### 3.1 JWT Secret in `appsettings.json` (Security)
**Problem:** `"SecretKey": "THIS_IS_SUPER_SECRET_KEY_1234567890_32BYTES"` is committed to source control.

**Fix — Use environment variables or User Secrets:**
```bash
# Development
dotnet user-secrets set "JwtSettings:SecretKey" "your-real-random-256bit-key"

# Production
$env:JwtSettings__SecretKey = "your-real-random-256bit-key"
```

Update `Program.cs` to read from config (already does — just move the value out of `appsettings.json`).  
Add to `.gitignore`: `appsettings.Development.json` if it contains real secrets.

---

### 3.2 `DependencyInjection.cs` Not Being Used
**Problem:** `AuthService.Infrastructure/DependencyInjection.cs` registers repositories, but `Program.cs` registers them manually again — duplication risk.

**Fix — Use the extension method in `Program.cs`:**
```csharp
// Program.cs — replace manual registrations with:
builder.Services.AddInfrastructure(builder.Configuration);
```

Then in `DependencyInjection.cs` add service registrations too:
```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<AuthDbContext>(opts =>
        opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IPermissionRepository, PermissionRepository>();
    services.AddScoped<IMenuRepository, MenuRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    return services;
}
```

---

### 3.3 Default Admin Password in Plaintext (`DbInitializer.cs`)
**Problem:** `"SuperAdmin@123"` is hardcoded in source code.

**Fix — Read from environment at seed time:**
```csharp
// DbInitializer.cs
public static async Task SeedAsync(AuthDbContext context, IConfiguration config)
{
    var adminPassword = config["SeedData:AdminPassword"]
        ?? throw new InvalidOperationException("SeedData:AdminPassword not configured");
    // ... use adminPassword
}
```

---

### 3.4 Commands/Queries Defined but Not Wired (CQRS Skeleton)
**Problem:** `Commands/` and `Queries/` folders exist (`CreateUserCommand`, `GetUserByIdQuery`, etc.) but are empty shells — services call repositories directly, bypassing them.

**Options:**
- **Option A (Simple):** Delete the `Commands/Queries` folders and keep service-based approach. Clean and honest.
- **Option B (Full CQRS):** Install MediatR and implement handlers.

```bash
# Option B
dotnet add AuthService.Application package MediatR
dotnet add AuthService.Application package MediatR.Extensions.Microsoft.DependencyInjection
```

```csharp
// Program.cs
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));
```

---

### 3.5 Role Authorization Not Enforced on `RoleController`
**Problem:** `RoleController` endpoints should be Admin-only but authorization is commented out.

**Fix:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]                          // require login for all
public class RoleController : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Super Admin")] // only admins can create roles
    public async Task<IActionResult> Create([FromBody] RoleCreateDto dto) { ... }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Delete(Guid id) { ... }
}
```

---

## 4. Architecture Improvements

### 4.1 Add Audit Fields to All Entities

Add a base entity class so every table tracks who created/modified records:

```csharp
// AuthService.Domain/Common/BaseEntity.cs
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
```

Override `SaveChangesAsync` in `AuthDbContext` to set these automatically:
```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    var now = DateTime.UtcNow;
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
        if (entry.State == EntityState.Added)
            entry.Entity.CreatedAt = now;
        if (entry.State is EntityState.Added or EntityState.Modified)
            entry.Entity.UpdatedAt = now;
    }
    return await base.SaveChangesAsync(ct);
}
```

---

### 4.2 Add Refresh Tokens

Current JWTs expire in 60 minutes with no way to renew without re-login.

**Domain entity:**
```csharp
// AuthService.Domain/Entities/RefreshToken.cs
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public User User { get; set; } = null!;
}
```

**New endpoint:**
```csharp
// AuthController
[HttpPost("refresh")]
[AllowAnonymous]
public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto) { ... }

[HttpPost("logout")]
[Authorize]
public async Task<IActionResult> Logout() { ... }   // revokes refresh token
```

---

### 4.3 Global API Response Wrapper

Currently some endpoints return raw objects, some return `ActionResult`. Standardise:

```csharp
// AuthService.Application/Common/ApiResponse.cs
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string? msg = null) =>
        new() { Success = true, Data = data, Message = msg };

    public static ApiResponse<T> Fail(string error) =>
        new() { Success = false, Errors = new[] { error } };
}
```

Controllers then return:
```csharp
return Ok(ApiResponse<UserReadDto>.Ok(user));
```

---

### 4.4 Pagination for List Endpoints

`GET /api/user` and `GET /api/role` return all records — a problem at scale.

```csharp
// AuthService.Application/Common/PagedResult.cs
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
```

Update repository interface:
```csharp
Task<PagedResult<User>> GetAllAsync(int page, int pageSize, string? search = null);
```

Endpoint signature:
```csharp
[HttpGet]
public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
```

---

### 4.5 Add FluentValidation

Currently there is no input validation — bad data reaches the service layer.

```bash
dotnet add AuthService.Application package FluentValidation.AspNetCore
```

```csharp
// AuthService.Application/Validators/UserCreateValidator.cs
public class UserCreateValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must contain an uppercase letter")
            .Matches(@"[0-9]").WithMessage("Must contain a digit");
        RuleFor(x => x.RoleIds).NotEmpty().WithMessage("At least one role required");
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddValidatorsFromAssembly(typeof(UserCreateValidator).Assembly);
```

---

### 4.6 Add Missing Menu API Endpoints

`MenuService` exists but there are no controller endpoints exposing it.

```csharp
// AuthService.API/Controllers/MenuController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    [HttpGet("my-menus")]
    public async Task<IActionResult> GetMyMenus()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var roleIds = User.FindAll(ClaimTypes.Role)
                         .Select(c => Guid.Parse(c.Value)).ToList();
        var menus = await _menuService.GetUserMenusAsync(userId, roleIds);
        return Ok(ApiResponse<IEnumerable<MenuReadDto>>.Ok(menus));
    }
}
```

---

### 4.7 Add Structured Logging (Serilog)

```bash
dotnet add AuthService.API package Serilog.AspNetCore
dotnet add AuthService.API package Serilog.Sinks.Console
dotnet add AuthService.API package Serilog.Sinks.File
```

```csharp
// Program.cs
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .Enrich.FromLogContext()
       .WriteTo.Console()
       .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));
```

---

### 4.8 Add Rate Limiting (Built-in .NET 7+)

```csharp
// Program.cs
builder.Services.AddRateLimiter(opts =>
{
    opts.AddFixedWindowLimiter("login", cfg =>
    {
        cfg.PermitLimit = 5;
        cfg.Window = TimeSpan.FromMinutes(1);
        cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        cfg.QueueLimit = 0;
    });
});

// AuthController.cs
[HttpPost("login")]
[EnableRateLimiting("login")]
[AllowAnonymous]
public async Task<IActionResult> Login([FromBody] LoginRequestDto dto) { ... }
```

---

### 4.9 Add Health Checks

```bash
dotnet add AuthService.API package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
```

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AuthDbContext>("database");

// Map endpoint
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/detail", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

---

### 4.10 Add Unit Tests

```bash
# In solution root
dotnet new xunit -n AuthService.Tests
dotnet add AuthService.Tests reference AuthService.Application
dotnet add AuthService.Tests package Moq
dotnet add AuthService.Tests package FluentAssertions
```

Example test:
```csharp
// AuthService.Tests/Services/UserServiceTests.cs
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IRoleRepository> _roleRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task ValidateUserAsync_WrongPassword_ReturnsNull()
    {
        _userRepo.Setup(r => r.GetByEmailAsync("test@example.com"))
                 .ReturnsAsync(new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct") });

        var svc = new UserService(_userRepo.Object, _roleRepo.Object, _uow.Object);
        var result = await svc.ValidateUserAsync("test@example.com", "wrong");

        result.Should().BeNull();
    }
}
```

---

## 5. API Versioning

```bash
dotnet add AuthService.API package Asp.Versioning.Mvc
dotnet add AuthService.API package Asp.Versioning.Mvc.ApiExplorer
```

```csharp
// Program.cs
builder.Services.AddApiVersioning(opts =>
{
    opts.DefaultApiVersion = new ApiVersion(1);
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.ReportApiVersions = true;
    opts.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Controller
[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController : ControllerBase { ... }
```

---

## 6. Environment Setup Guide

### Prerequisites
- .NET 9 SDK
- SQL Server (local or Docker)
- Visual Studio 2022 / VS Code / Rider

### First-Time Setup
```bash
# 1. Clone repo
git clone <repo-url>
cd AdminMicroservices

# 2. Set secrets (never put real values in appsettings.json)
cd AuthService.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=AdminAuth;Trusted_Connection=True;"
dotnet user-secrets set "JwtSettings:SecretKey" "<generate-a-random-32+-char-key>"

# 3. Run migrations
dotnet ef database update --project AuthService.Infrastructure --startup-project AuthService.API

# 4. Run
dotnet run --project AuthService.API
# Swagger: http://localhost:5222/swagger
```

### Docker Compose (Recommended)
```yaml
# docker-compose.yml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: YourStr0ngP@ssword
    ports:
      - "1433:1433"

  authservice:
    build: .
    environment:
      ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=AdminAuth;User Id=sa;Password=YourStr0ngP@ssword;TrustServerCertificate=True;"
      JwtSettings__SecretKey: "your-random-key-here"
    ports:
      - "5222:8080"
    depends_on:
      - sqlserver
```

---

## 7. Improvement Priority Matrix

| Priority | Issue / Feature                        | Effort | Impact |
|----------|----------------------------------------|--------|--------|
| P0       | Move JWT secret out of appsettings     | Low    | High   |
| P0       | Move admin seed password to env vars   | Low    | High   |
| P0       | Enforce role auth on RoleController    | Low    | High   |
| P1       | Use `DependencyInjection.cs` extension | Low    | Medium |
| P1       | Add FluentValidation                   | Medium | High   |
| P1       | Add Menu API endpoints                 | Low    | High   |
| P1       | Add audit fields (BaseEntity)          | Medium | High   |
| P2       | Add pagination to list endpoints       | Medium | High   |
| P2       | Add refresh token support              | Medium | High   |
| P2       | Add global response wrapper            | Low    | Medium |
| P2       | Add rate limiting on login             | Low    | High   |
| P3       | Serilog structured logging             | Low    | Medium |
| P3       | Health checks endpoint                 | Low    | Medium |
| P3       | Unit tests (xUnit + Moq)               | High   | High   |
| P3       | API versioning                         | Medium | Medium |
| P3       | Decide: keep or implement CQRS/MediatR | High   | Medium |

---

## 8. Folder Structure After Full Improvements

```
AuthService.Application/
├── Common/
│   ├── ApiResponse.cs         ← NEW: standard response wrapper
│   ├── BaseEntity.cs          ← NEW: audit fields
│   └── PagedResult.cs         ← NEW: pagination
├── Validators/                ← NEW: FluentValidation validators
│   ├── UserCreateValidator.cs
│   ├── UserUpdateValidator.cs
│   └── RoleCreateValidator.cs
├── Commands/                  ← EITHER implement with MediatR or remove
├── Queries/                   ← EITHER implement with MediatR or remove
├── DTOs/                      (existing)
├── Interfaces/                (existing)
└── Services/                  (existing)

AuthService.Domain/
├── Common/
│   └── BaseEntity.cs          ← NEW
├── Entities/
│   └── RefreshToken.cs        ← NEW
└── ... (existing)

AuthService.API/
├── Controllers/
│   └── MenuController.cs      ← NEW
└── ... (existing)

AuthService.Tests/             ← NEW: entire test project
├── Services/
│   ├── UserServiceTests.cs
│   └── RoleServiceTests.cs
└── Controllers/
    └── AuthControllerTests.cs
```

---

## 9. Security Checklist

- [ ] JWT secret rotated and stored in env/secrets manager
- [ ] Seed password stored in env/secrets manager
- [ ] HTTPS enforced in production (`UseHttpsRedirection`)
- [ ] CORS locked to known frontend origins only
- [ ] Rate limiting on auth endpoints
- [ ] Role-based authorization on all sensitive endpoints
- [ ] Password complexity enforced via FluentValidation
- [ ] Refresh tokens revocable on logout
- [ ] No sensitive data in logs (passwords, tokens)
- [ ] EF parameterized queries used throughout (EF Core does this by default)

---

## 10. Recommended Reading / References

- [Clean Architecture — Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core JWT Auth Docs](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [EF Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [FluentValidation Docs](https://docs.fluentvalidation.net/en/latest/)
- [MediatR Docs](https://github.com/jbogard/MediatR/wiki)
- [Serilog for ASP.NET Core](https://github.com/serilog/serilog-aspnetcore)
