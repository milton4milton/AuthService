using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AuthDbContext context, IConfiguration config)
    {
        if (await context.Roles.AnyAsync()) return;

        var adminPassword = config["SeedData:AdminPassword"]
            ?? throw new InvalidOperationException(
                "SeedData:AdminPassword is not configured. " +
                "Set it via user-secrets: dotnet user-secrets set \"SeedData:AdminPassword\" \"<password>\"");

        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Super Admin",
            Description = "Super Admin"
        };

        await context.Roles.AddAsync(adminRole);
        await context.SaveChangesAsync();

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "superadmin",
            Email = "superadmin@admin.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            IsActive = true
        };

        adminUser.UserRoles.Add(new UserRole
        {
            RoleId = adminRole.Id,
            UserId = adminUser.Id
        });

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}
