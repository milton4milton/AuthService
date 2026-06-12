using AccountingService.Application.Common;
using AccountingService.Application.DTOs.FinancialYear;

namespace AccountingService.Application.Interfaces;

public interface IFinancialYearService
{
    Task<FinancialYearReadDto?> GetByIdAsync(Guid id);
    Task<List<FinancialYearReadDto>> GetAllByOrganizationAsync(Guid organizationId);
    Task<PagedResult<FinancialYearReadDto>> GetPagedAsync(Guid organizationId, int page, int pageSize);
    Task<FinancialYearReadDto> CreateAsync(FinancialYearCreateDto dto);
    Task<FinancialYearReadDto?> UpdateAsync(Guid id, FinancialYearUpdateDto dto);
    Task<FinancialYearReadDto?> CloseAsync(Guid id);
    Task DeleteAsync(Guid id);
}
