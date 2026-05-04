using AuthService.Application.DTOs.User;

namespace AuthService.Application.Interfaces;

public interface IUserService
{
    // Query: Get user by Id
    Task<UserReadDto> GetByIdAsync(Guid id);

    // Command: Create user
    Task<UserReadDto> CreateUserAsync(UserCreateDto dto);

    // Command: Update user
    Task<UserReadDto> UpdateUserAsync(Guid userId, UserUpdateDto dto);

    // Optional: List all users
    Task<List<UserReadDto>> GetAllAsync();
    Task DeleteUserAsync(Guid id);
    // Add this method
    Task<UserReadDto?> ValidateUserAsync(string email, string password);




  



}
