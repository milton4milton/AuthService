using AuthService.Application.DTOs.Role;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoleReadDto> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) throw new Exception("Role not found");

        return new RoleReadDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive
        };
    }

    public async Task<List<RoleReadDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(r => new RoleReadDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsActive = r.IsActive
        }).ToList();
    }

    public async Task<RoleReadDto> CreateRoleAsync(RoleCreateDto dto)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        await _roleRepository.AddAsync(role); // Implement AddAsync in IRoleRepository
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(role.Id);
    }
    public async Task DeleteRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new Exception("Role not found");

        _roleRepository.Delete(role);
        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<RoleReadDto> UpdateRoleAsync(Guid roleId, RoleUpdateDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null) throw new Exception("Role not found");

        if (!string.IsNullOrEmpty(dto.Name)) role.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Description)) role.Description = dto.Description;
        if (dto.IsActive.HasValue) role.IsActive = dto.IsActive.Value;

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(role.Id);
    }
}
