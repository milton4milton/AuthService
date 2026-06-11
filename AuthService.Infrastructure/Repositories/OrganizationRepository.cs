using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly AuthDbContext _context;

    public OrganizationRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(Guid id)
        => await _context.Organizations
            .Include(o => o.Branches)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<Organization>> GetAllAsync()
        => await _context.Organizations
            .Include(o => o.Branches)
            .OrderBy(o => o.Name)
            .ToListAsync();

    public async Task<(List<Organization> Data, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Organizations.OrderBy(o => o.Name);
        var totalCount = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task AddAsync(Organization organization)
        => await _context.Organizations.AddAsync(organization);

    public void Update(Organization organization)
        => _context.Organizations.Update(organization);

    public void Delete(Organization organization)
        => _context.Organizations.Remove(organization);
}
