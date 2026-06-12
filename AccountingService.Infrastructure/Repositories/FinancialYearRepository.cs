using AccountingService.Domain.Entities;
using AccountingService.Domain.Interfaces;
using AccountingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AccountingService.Infrastructure.Repositories;

public class FinancialYearRepository : IFinancialYearRepository
{
    private readonly AccountingDbContext _context;

    public FinancialYearRepository(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialYear?> GetByIdAsync(Guid id)
        => await _context.FinancialYears.FirstOrDefaultAsync(f => f.Id == id);

    public async Task<List<FinancialYear>> GetAllByOrganizationAsync(Guid organizationId)
        => await _context.FinancialYears
            .Where(f => f.OrganizationId == organizationId)
            .OrderByDescending(f => f.StartDate)
            .ToListAsync();

    public async Task<(List<FinancialYear> Data, int TotalCount)> GetPagedAsync(Guid organizationId, int page, int pageSize)
    {
        var query = _context.FinancialYears
            .Where(f => f.OrganizationId == organizationId)
            .OrderByDescending(f => f.StartDate);

        var totalCount = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task AddAsync(FinancialYear entity)
        => await _context.FinancialYears.AddAsync(entity);

    public void Update(FinancialYear entity)
        => _context.FinancialYears.Update(entity);

    public void Delete(FinancialYear entity)
        => _context.FinancialYears.Remove(entity);
}
