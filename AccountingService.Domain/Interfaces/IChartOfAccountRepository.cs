using AccountingService.Domain.Entities;

namespace AccountingService.Domain.Interfaces;

public interface IChartOfAccountRepository
{
    Task<ChartOfAccount?> GetByIdAsync(Guid id);
    Task<List<ChartOfAccount>> GetAllByOrganizationAsync(Guid organizationId);
    Task<List<ChartOfAccount>> GetChildrenAsync(Guid organizationId, Guid? parentId);
    Task<(List<ChartOfAccount> Data, int TotalCount)> GetPagedAsync(Guid organizationId, Guid? parentId, int page, int pageSize);
    Task<bool> HasChildrenAsync(Guid id);
    Task AddAsync(ChartOfAccount account);
    void Update(ChartOfAccount account);
    void Delete(ChartOfAccount account);
}
