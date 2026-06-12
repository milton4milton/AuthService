using AccountingService.Application.Common;
using AccountingService.Application.DTOs.ChartOfAccount;

namespace AccountingService.Application.Interfaces;

public interface IChartOfAccountService
{
    Task<ChartOfAccountReadDto?> GetByIdAsync(Guid id);
    Task<List<ChartOfAccountReadDto>> GetAllByOrganizationAsync(Guid organizationId);
    Task<List<ChartOfAccountTreeDto>> GetTreeAsync(Guid organizationId);
    Task<List<ChartOfAccountReadDto>> GetChildrenAsync(Guid organizationId, Guid? parentId);
    Task<PagedResult<ChartOfAccountReadDto>> GetPagedAsync(Guid organizationId, Guid? parentId, int page, int pageSize);
    Task<ChartOfAccountReadDto> CreateAsync(ChartOfAccountCreateDto dto);
    Task<ChartOfAccountReadDto?> UpdateAsync(Guid id, ChartOfAccountUpdateDto dto);
    Task DeleteAsync(Guid id);
}
