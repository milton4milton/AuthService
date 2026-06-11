using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<(List<Role> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);
    void Delete(Role role);
    Task AddAsync(Role role);
}
