using AuthService.Application.Common;
using AuthService.Application.DTOs.Branch;

namespace AuthService.Application.Interfaces;

public interface IBranchService
{
    Task<BranchReadDto?> GetByIdAsync(Guid id);
    Task<List<BranchReadDto>> GetAllAsync();
    Task<PagedResult<BranchReadDto>> GetPagedAsync(int page, int pageSize);
    Task<List<BranchReadDto>> GetByOrganizationIdAsync(Guid organizationId);
    Task<BranchReadDto> CreateAsync(BranchCreateDto dto);
    Task<BranchReadDto?> UpdateAsync(Guid id, BranchUpdateDto dto);
    Task DeleteAsync(Guid id);
}
