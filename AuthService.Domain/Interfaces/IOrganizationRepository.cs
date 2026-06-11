using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id);
    Task<List<Organization>> GetAllAsync();
    Task<(List<Organization> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task AddAsync(Organization organization);
    void Update(Organization organization);
    void Delete(Organization organization);
}
