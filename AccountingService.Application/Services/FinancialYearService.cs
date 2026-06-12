using AccountingService.Application.Common;
using AccountingService.Application.DTOs.FinancialYear;
using AccountingService.Application.Interfaces;
using AccountingService.Domain.Entities;
using AccountingService.Domain.Interfaces;

namespace AccountingService.Application.Services;

public class FinancialYearService : IFinancialYearService
{
    private readonly IFinancialYearRepository _repo;
    private readonly IUnitOfWork _uow;

    public FinancialYearService(IFinancialYearRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<FinancialYearReadDto?> GetByIdAsync(Guid id)
    {
        var fy = await _repo.GetByIdAsync(id);
        return fy is null ? null : MapToDto(fy);
    }

    public async Task<List<FinancialYearReadDto>> GetAllByOrganizationAsync(Guid organizationId)
    {
        var list = await _repo.GetAllByOrganizationAsync(organizationId);
        return list.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<FinancialYearReadDto>> GetPagedAsync(Guid organizationId, int page, int pageSize)
    {
        var (data, totalCount) = await _repo.GetPagedAsync(organizationId, page, pageSize);
        return new PagedResult<FinancialYearReadDto>
        {
            Data = data.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<FinancialYearReadDto> CreateAsync(FinancialYearCreateDto dto)
    {
        var entity = new FinancialYear
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<FinancialYearReadDto?> UpdateAsync(Guid id, FinancialYearUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return null;

        if (dto.Name is not null) entity.Name = dto.Name;
        if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
        if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await _uow.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<FinancialYearReadDto?> CloseAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return null;

        entity.IsClosed = true;
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await _uow.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return;

        _repo.Delete(entity);
        await _uow.SaveChangesAsync();
    }

    private static FinancialYearReadDto MapToDto(FinancialYear fy) => new()
    {
        Id = fy.Id,
        OrganizationId = fy.OrganizationId,
        Name = fy.Name,
        StartDate = fy.StartDate,
        EndDate = fy.EndDate,
        IsActive = fy.IsActive,
        IsClosed = fy.IsClosed,
        CreatedAt = fy.CreatedAt,
        UpdatedAt = fy.UpdatedAt
    };
}
