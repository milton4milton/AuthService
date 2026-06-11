using AuthService.Application.Common;
using AuthService.Application.DTOs.User;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserReadDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new Exception("User not found");
        return MapToDto(user);
    }

    public async Task<List<UserReadDto>> GetAllAsync()
        => (await _userRepository.GetAllAsync()).Select(MapToDto).ToList();

    public async Task<PagedResult<UserReadDto>> GetPagedAsync(int page, int pageSize)
    {
        var (data, totalCount) = await _userRepository.GetPagedAsync(page, pageSize);
        return new PagedResult<UserReadDto>
        {
            Data = data.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<List<UserReadDto>> GetByOrganizationIdAsync(Guid organizationId)
        => (await _userRepository.GetByOrganizationIdAsync(organizationId)).Select(MapToDto).ToList();

    public async Task<List<UserReadDto>> GetByBranchIdAsync(Guid branchId)
        => (await _userRepository.GetByBranchIdAsync(branchId)).Select(MapToDto).ToList();

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsActive = true,
            OrganizationId = dto.OrganizationId,
            BranchId = dto.BranchId
        };

        foreach (var roleId in dto.RoleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) continue;
            user.UserRoles.Add(new UserRole { RoleId = role.Id, UserId = user.Id });
        }

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(user.Id);
    }

    public async Task<UserReadDto> UpdateUserAsync(Guid userId, UserUpdateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        if (!string.IsNullOrWhiteSpace(dto.UserName)) user.UserName = dto.UserName;
        if (!string.IsNullOrWhiteSpace(dto.Email)) user.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
        if (dto.OrganizationId.HasValue) user.OrganizationId = dto.OrganizationId.Value;
        if (dto.BranchId.HasValue) user.BranchId = dto.BranchId.Value;

        if (dto.RoleIds != null)
        {
            user.UserRoles.Clear();
            foreach (var roleId in dto.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null) continue;
                user.UserRoles.Add(new UserRole { RoleId = role.Id, UserId = user.Id });
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new Exception("User not found");
        _userRepository.Delete(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<UserReadDto?> ValidateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
        return MapToDto(user);
    }

    private static UserReadDto MapToDto(User u) => new()
    {
        Id = u.Id,
        UserName = u.UserName,
        Email = u.Email,
        IsActive = u.IsActive,
        Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
        OrganizationId = u.OrganizationId,
        OrganizationName = u.Organization?.Name,
        BranchId = u.BranchId,
        BranchName = u.Branch?.Name
    };
}
