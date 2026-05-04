using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<Menu>> GetMenusByPermissionsAsync(IEnumerable<Guid> permissionIds);
}
