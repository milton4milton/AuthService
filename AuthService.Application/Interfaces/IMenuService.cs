using AuthService.Application.DTOs.Menu;

namespace AuthService.Application.Interfaces;

public interface IMenuService
{
    Task<List<MenuReadDto>> GetUserMenusAsync(Guid userId, List<Guid> roleIds);
}
