using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class MenuRepository : IMenuRepository
{
    private readonly AuthDbContext _context;

    public MenuRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Menu>> GetMenusByPermissionsAsync(IEnumerable<Guid> permissionIds)
    {
        return await _context.Menus
            .Where(x => permissionIds.Contains(x.PermissionId))
            .OrderBy(x => x.OrderNo)
            .ToListAsync();
    }
}
