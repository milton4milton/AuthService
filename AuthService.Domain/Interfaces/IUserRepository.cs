using AuthService.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    // Task GetByEmailAsync(string email);
    void Delete(User user);
    Task<User?> GetByEmailAsync(string email);
}
