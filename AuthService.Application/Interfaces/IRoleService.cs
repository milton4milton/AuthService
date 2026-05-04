using AuthService.Application.DTOs.Role;

namespace AuthService.Application.Interfaces;

public interface IRoleService
{
    Task<RoleReadDto> GetByIdAsync(Guid id);
    Task<List<RoleReadDto>> GetAllAsync();
    Task<RoleReadDto> CreateRoleAsync(RoleCreateDto dto);
    Task DeleteRoleAsync(Guid id);
    Task<RoleReadDto> UpdateRoleAsync(Guid roleId, RoleUpdateDto dto);
}
