
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AuthDbContext context)
    {
        // Already seeded?
        if (await context.Roles.AnyAsync()) return;

        // 1️⃣ Create Admin Role
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Super Admin",
            Description="Super Admin"
        };

        await context.Roles.AddAsync(adminRole);

        // Save Roles first! (Important)
        await context.SaveChangesAsync();

        // 2️⃣ Create Default Admin User
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "superadmin",
            Email = "superadmin@admin.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"),
            IsActive = true
        };

        // 3️⃣ Assign Admin role
        adminUser.UserRoles.Add(new UserRole
        {
            RoleId = adminRole.Id,
            UserId = adminUser.Id
        });

        await context.Users.AddAsync(adminUser);

        // Save User
        await context.SaveChangesAsync();
    }
}

