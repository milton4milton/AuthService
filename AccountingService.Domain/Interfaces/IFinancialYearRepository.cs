using AccountingService.Domain.Entities;

namespace AccountingService.Domain.Interfaces;

public interface IFinancialYearRepository
{
    Task<FinancialYear?> GetByIdAsync(Guid id);
    Task<List<FinancialYear>> GetAllByOrganizationAsync(Guid organizationId);
    Task<(List<FinancialYear> Data, int TotalCount)> GetPagedAsync(Guid organizationId, int page, int pageSize);
    Task AddAsync(FinancialYear financialYear);
    void Update(FinancialYear financialYear);
    void Delete(FinancialYear financialYear);
}
