using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(Guid id);
    Task<List<Branch>> GetAllAsync();
    Task<(List<Branch> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<List<Branch>> GetByOrganizationIdAsync(Guid organizationId);
    Task AddAsync(Branch branch);
    void Update(Branch branch);
    void Delete(Branch branch);
}
