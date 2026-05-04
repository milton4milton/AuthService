using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId);
}
