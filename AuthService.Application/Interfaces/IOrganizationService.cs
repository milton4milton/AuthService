using AuthService.Application.Common;
using AuthService.Application.DTOs.Organization;

namespace AuthService.Application.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationReadDto?> GetByIdAsync(Guid id);
    Task<List<OrganizationReadDto>> GetAllAsync();
    Task<PagedResult<OrganizationReadDto>> GetPagedAsync(int page, int pageSize);
    Task<OrganizationReadDto> CreateAsync(OrganizationCreateDto dto);
    Task<OrganizationReadDto?> UpdateAsync(Guid id, OrganizationUpdateDto dto);
    Task DeleteAsync(Guid id);
}
