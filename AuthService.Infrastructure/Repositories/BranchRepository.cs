using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly AuthDbContext _context;

    public BranchRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Branch?> GetByIdAsync(Guid id)
        => await _context.Branches
            .Include(b => b.Organization)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<List<Branch>> GetAllAsync()
        => await _context.Branches
            .Include(b => b.Organization)
            .OrderBy(b => b.Organization.Name).ThenBy(b => b.Name)
            .ToListAsync();

    public async Task<(List<Branch> Data, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Branches
            .Include(b => b.Organization)
            .OrderBy(b => b.Organization.Name).ThenBy(b => b.Name);
        var totalCount = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task<List<Branch>> GetByOrganizationIdAsync(Guid organizationId)
        => await _context.Branches
            .Include(b => b.Organization)
            .Where(b => b.OrganizationId == organizationId)
            .OrderBy(b => b.Name)
            .ToListAsync();

    public async Task AddAsync(Branch branch)
        => await _context.Branches.AddAsync(branch);

    public void Update(Branch branch)
        => _context.Branches.Update(branch);

    public void Delete(Branch branch)
        => _context.Branches.Remove(branch);
}
