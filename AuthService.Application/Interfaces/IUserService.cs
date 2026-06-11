using AuthService.Application.Common;
using AuthService.Application.DTOs.User;

namespace AuthService.Application.Interfaces;

public interface IUserService
{
    Task<UserReadDto> GetByIdAsync(Guid id);
    Task<List<UserReadDto>> GetAllAsync();
    Task<PagedResult<UserReadDto>> GetPagedAsync(int page, int pageSize);
    Task<List<UserReadDto>> GetByOrganizationIdAsync(Guid organizationId);
    Task<List<UserReadDto>> GetByBranchIdAsync(Guid branchId);
    Task<UserReadDto> CreateUserAsync(UserCreateDto dto);
    Task<UserReadDto> UpdateUserAsync(Guid userId, UserUpdateDto dto);
    Task DeleteUserAsync(Guid id);
    Task<UserReadDto?> ValidateUserAsync(string email, string password);
}
