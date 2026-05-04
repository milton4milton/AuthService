using AuthService.Application.DTOs.User;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new Exception("User not found");

        return new UserReadDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }

    public async Task<List<UserReadDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync(); // Implement in repository
        return users.Select(u => new UserReadDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            IsActive = u.IsActive,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        }).ToList();
    }

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = passwordHash,
            IsActive = true
        };

        // Assign roles
        foreach (var roleId in dto.RoleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) continue;

            user.UserRoles.Add(new UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            });
        }

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(user.Id);
    }

    public async Task<UserReadDto> UpdateUserAsync(Guid userId, UserUpdateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        if (!string.IsNullOrEmpty(dto.UserName)) user.UserName = dto.UserName;
        if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Password)) user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

        // Update Roles
        if (dto.RoleIds != null)
        {
            user.UserRoles.Clear();
            foreach (var roleId in dto.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null) continue;

                user.UserRoles.Add(new UserRole
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }


    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new Exception("User not found");

        _userRepository.Delete(user); // Add Delete method in repository
        await _unitOfWork.SaveChangesAsync();
    }


    public async Task<UserReadDto?> ValidateUserAsync(string email, string password)
    {
        // Get user by email (repository method)
        var user = await _userRepository.GetByEmailAsync(email); // Fix: Ensure GetByEmailAsync returns Task<User?> instead of Task<User>
        if (user == null) return null;

        // Verify password using BCrypt
        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isValid) return null;

        // Map to DTO
        return new UserReadDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }

}
