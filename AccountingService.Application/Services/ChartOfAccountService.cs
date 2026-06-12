using AccountingService.Application.Common;
using AccountingService.Application.DTOs.ChartOfAccount;
using AccountingService.Application.Interfaces;
using AccountingService.Domain.Entities;
using AccountingService.Domain.Interfaces;

namespace AccountingService.Application.Services;

public class ChartOfAccountService : IChartOfAccountService
{
    private readonly IChartOfAccountRepository _repo;
    private readonly IUnitOfWork _uow;

    public ChartOfAccountService(IChartOfAccountRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<ChartOfAccountReadDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity is null ? null : MapToReadDto(entity);
    }

    public async Task<List<ChartOfAccountReadDto>> GetAllByOrganizationAsync(Guid organizationId)
    {
        var list = await _repo.GetAllByOrganizationAsync(organizationId);
        return list.Select(MapToReadDto).ToList();
    }

    public async Task<List<ChartOfAccountTreeDto>> GetTreeAsync(Guid organizationId)
    {
        var all = await _repo.GetAllByOrganizationAsync(organizationId);
        return BuildTree(all, null);
    }

    public async Task<List<ChartOfAccountReadDto>> GetChildrenAsync(Guid organizationId, Guid? parentId)
    {
        var list = await _repo.GetChildrenAsync(organizationId, parentId);
        return list.Select(MapToReadDto).ToList();
    }

    public async Task<PagedResult<ChartOfAccountReadDto>> GetPagedAsync(Guid organizationId, Guid? parentId, int page, int pageSize)
    {
        var (data, totalCount) = await _repo.GetPagedAsync(organizationId, parentId, page, pageSize);
        return new PagedResult<ChartOfAccountReadDto>
        {
            Data = data.Select(MapToReadDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ChartOfAccountReadDto> CreateAsync(ChartOfAccountCreateDto dto)
    {
        int level = 0;
        if (dto.ParentId.HasValue)
        {
            var parent = await _repo.GetByIdAsync(dto.ParentId.Value);
            level = parent is not null ? parent.Level + 1 : 0;
        }

        var entity = new ChartOfAccount
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            ParentId = dto.ParentId,
            Code = dto.Code,
            Name = dto.Name,
            AccountType = dto.AccountType,
            Nature = dto.Nature,
            IsGroup = dto.IsGroup,
            Level = level,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var created = await _repo.GetByIdAsync(entity.Id);
        return MapToReadDto(created!);
    }

    public async Task<ChartOfAccountReadDto?> UpdateAsync(Guid id, ChartOfAccountUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return null;

        if (dto.Name is not null) entity.Name = dto.Name;
        if (dto.AccountType.HasValue) entity.AccountType = dto.AccountType.Value;
        if (dto.Nature.HasValue) entity.Nature = dto.Nature.Value;
        if (dto.Description is not null) entity.Description = dto.Description;
        if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        _repo.Update(entity);
        await _uow.SaveChangesAsync();

        var updated = await _repo.GetByIdAsync(id);
        return MapToReadDto(updated!);
    }

    public async Task DeleteAsync(Guid id)
    {
        var hasChildren = await _repo.HasChildrenAsync(id);
        if (hasChildren)
            throw new InvalidOperationException("Cannot delete an account that has child accounts.");

        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return;

        _repo.Delete(entity);
        await _uow.SaveChangesAsync();
    }

    private static List<ChartOfAccountTreeDto> BuildTree(List<ChartOfAccount> all, Guid? parentId)
    {
        return all
            .Where(a => a.ParentId == parentId)
            .OrderBy(a => a.Code)
            .Select(a => new ChartOfAccountTreeDto
            {
                Id = a.Id,
                OrganizationId = a.OrganizationId,
                ParentId = a.ParentId,
                Code = a.Code,
                Name = a.Name,
                AccountType = a.AccountType.ToString(),
                Nature = a.Nature.ToString(),
                IsGroup = a.IsGroup,
                Level = a.Level,
                Description = a.Description,
                IsActive = a.IsActive,
                Children = BuildTree(all, a.Id)
            })
            .ToList();
    }

    private static ChartOfAccountReadDto MapToReadDto(ChartOfAccount a) => new()
    {
        Id = a.Id,
        OrganizationId = a.OrganizationId,
        ParentId = a.ParentId,
        ParentCode = a.Parent?.Code,
        ParentName = a.Parent?.Name,
        Code = a.Code,
        Name = a.Name,
        AccountType = a.AccountType.ToString(),
        Nature = a.Nature.ToString(),
        IsGroup = a.IsGroup,
        Level = a.Level,
        Description = a.Description,
        IsActive = a.IsActive,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
