using AuthService.Application.DTOs.Menu;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;
    private readonly IPermissionRepository _permissionRepository;

    public MenuService(IMenuRepository menuRepository, IPermissionRepository permissionRepository)
    {
        _menuRepository = menuRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<List<MenuReadDto>> GetUserMenusAsync(Guid userId, List<Guid> roleIds)
    {
        var permissionIds = new List<Guid>();
        foreach (var roleId in roleIds)
        {
            var perms = await _permissionRepository.GetByRoleIdAsync(roleId);
            permissionIds.AddRange(perms.Select(p => p.Id));
        }

        var menus = await _menuRepository.GetMenusByPermissionsAsync(permissionIds.Distinct());

        return menus.Select(m => new MenuReadDto
        {
            Id = m.Id,
            Title = m.Title,
            Url = m.Url,
            ParentId = m.ParentId
        }).ToList();
    }
}
