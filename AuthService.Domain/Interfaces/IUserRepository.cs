using AuthService.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<(List<User> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<List<User>> GetByOrganizationIdAsync(Guid organizationId);
    Task<List<User>> GetByBranchIdAsync(Guid branchId);
    Task AddAsync(User user);
    void Delete(User user);
}
