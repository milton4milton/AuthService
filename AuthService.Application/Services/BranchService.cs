using AuthService.Application.Common;
using AuthService.Application.DTOs.Branch;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _branchRepository;
    private readonly IOrganizationRepository _orgRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BranchService(
        IBranchRepository branchRepository,
        IOrganizationRepository orgRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _orgRepository = orgRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BranchReadDto?> GetByIdAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        return branch == null ? null : MapToDto(branch);
    }

    public async Task<List<BranchReadDto>> GetAllAsync()
    {
        var branches = await _branchRepository.GetAllAsync();
        return branches.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<BranchReadDto>> GetPagedAsync(int page, int pageSize)
    {
        var (data, totalCount) = await _branchRepository.GetPagedAsync(page, pageSize);
        return new PagedResult<BranchReadDto>
        {
            Data = data.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<List<BranchReadDto>> GetByOrganizationIdAsync(Guid organizationId)
    {
        var branches = await _branchRepository.GetByOrganizationIdAsync(organizationId);
        return branches.Select(MapToDto).ToList();
    }

    public async Task<BranchReadDto> CreateAsync(BranchCreateDto dto)
    {
        var org = await _orgRepository.GetByIdAsync(dto.OrganizationId)
            ?? throw new Exception("Organization not found");

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            Name = dto.Name,
            Code = dto.Code.ToUpper(),
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _branchRepository.AddAsync(branch);
        await _unitOfWork.SaveChangesAsync();

        branch.Organization = org;
        return MapToDto(branch);
    }

    public async Task<BranchReadDto?> UpdateAsync(Guid id, BranchUpdateDto dto)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        if (branch == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name)) branch.Name = dto.Name;
        if (dto.Email != null) branch.Email = dto.Email;
        if (dto.Phone != null) branch.Phone = dto.Phone;
        if (dto.Address != null) branch.Address = dto.Address;
        if (dto.IsActive.HasValue) branch.IsActive = dto.IsActive.Value;
        branch.UpdatedAt = DateTime.UtcNow;

        _branchRepository.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(branch);
    }

    public async Task DeleteAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);
        if (branch == null) throw new Exception("Branch not found");

        _branchRepository.Delete(branch);
        await _unitOfWork.SaveChangesAsync();
    }

    private static BranchReadDto MapToDto(Branch b) => new()
    {
        Id = b.Id,
        OrganizationId = b.OrganizationId,
        OrganizationName = b.Organization?.Name ?? string.Empty,
        Name = b.Name,
        Code = b.Code,
        Email = b.Email,
        Phone = b.Phone,
        Address = b.Address,
        IsActive = b.IsActive,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    };
}
