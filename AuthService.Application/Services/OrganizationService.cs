using AuthService.Application.Common;
using AuthService.Application.DTOs.Organization;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _orgRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationService(IOrganizationRepository orgRepository, IUnitOfWork unitOfWork)
    {
        _orgRepository = orgRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrganizationReadDto?> GetByIdAsync(Guid id)
    {
        var org = await _orgRepository.GetByIdAsync(id);
        return org == null ? null : MapToDto(org);
    }

    public async Task<List<OrganizationReadDto>> GetAllAsync()
    {
        var orgs = await _orgRepository.GetAllAsync();
        return orgs.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<OrganizationReadDto>> GetPagedAsync(int page, int pageSize)
    {
        var (data, totalCount) = await _orgRepository.GetPagedAsync(page, pageSize);
        return new PagedResult<OrganizationReadDto>
        {
            Data = data.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<OrganizationReadDto> CreateAsync(OrganizationCreateDto dto)
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code.ToUpper(),
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            LogoUrl = dto.LogoUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _orgRepository.AddAsync(org);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(org);
    }

    public async Task<OrganizationReadDto?> UpdateAsync(Guid id, OrganizationUpdateDto dto)
    {
        var org = await _orgRepository.GetByIdAsync(id);
        if (org == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name)) org.Name = dto.Name;
        if (dto.Email != null) org.Email = dto.Email;
        if (dto.Phone != null) org.Phone = dto.Phone;
        if (dto.Address != null) org.Address = dto.Address;
        if (dto.LogoUrl != null) org.LogoUrl = dto.LogoUrl;
        if (dto.IsActive.HasValue) org.IsActive = dto.IsActive.Value;
        org.UpdatedAt = DateTime.UtcNow;

        _orgRepository.Update(org);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(org);
    }

    public async Task DeleteAsync(Guid id)
    {
        var org = await _orgRepository.GetByIdAsync(id);
        if (org == null) throw new Exception("Organization not found");

        _orgRepository.Delete(org);
        await _unitOfWork.SaveChangesAsync();
    }

    private static OrganizationReadDto MapToDto(Organization org) => new()
    {
        Id = org.Id,
        Name = org.Name,
        Code = org.Code,
        Email = org.Email,
        Phone = org.Phone,
        Address = org.Address,
        LogoUrl = org.LogoUrl,
        IsActive = org.IsActive,
        CreatedAt = org.CreatedAt,
        UpdatedAt = org.UpdatedAt
    };
}
