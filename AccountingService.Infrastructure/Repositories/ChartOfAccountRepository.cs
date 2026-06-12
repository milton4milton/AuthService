using AccountingService.Domain.Entities;
using AccountingService.Domain.Interfaces;
using AccountingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AccountingService.Infrastructure.Repositories;

public class ChartOfAccountRepository : IChartOfAccountRepository
{
    private readonly AccountingDbContext _context;

    public ChartOfAccountRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<ChartOfAccount?> GetByIdAsync(Guid id)
        => await _context.ChartOfAccounts
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<ChartOfAccount>> GetAllByOrganizationAsync(Guid organizationId)
        => await _context.ChartOfAccounts
            .Include(c => c.Parent)
            .Where(c => c.OrganizationId == organizationId)
            .OrderBy(c => c.Code)
            .ToListAsync();

    public async Task<List<ChartOfAccount>> GetChildrenAsync(Guid organizationId, Guid? parentId)
        => await _context.ChartOfAccounts
            .Include(c => c.Parent)
            .Where(c => c.OrganizationId == organizationId && c.ParentId == parentId)
            .OrderBy(c => c.Code)
            .ToListAsync();

    public async Task<(List<ChartOfAccount> Data, int TotalCount)> GetPagedAsync(Guid organizationId, Guid? parentId, int page, int pageSize)
    {
        var query = _context.ChartOfAccounts
            .Include(c => c.Parent)
            .Where(c => c.OrganizationId == organizationId && c.ParentId == parentId)
            .OrderBy(c => c.Code);

        var totalCount = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task<bool> HasChildrenAsync(Guid id)
        => await _context.ChartOfAccounts.AnyAsync(c => c.ParentId == id);

    public async Task AddAsync(ChartOfAccount entity)
        => await _context.ChartOfAccounts.AddAsync(entity);

    public void Update(ChartOfAccount entity)
        => _context.ChartOfAccounts.Update(entity);

    public void Delete(ChartOfAccount entity)
        => _context.ChartOfAccounts.Remove(entity);
}
