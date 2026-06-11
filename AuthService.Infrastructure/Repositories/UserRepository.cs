using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Organization)
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Organization)
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<List<User>> GetAllAsync()
        => await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Organization)
            .Include(u => u.Branch)
            .ToListAsync();

    public async Task<(List<User> Data, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Organization)
            .Include(u => u.Branch)
            .OrderBy(u => u.UserName);
        var totalCount = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (data, totalCount);
    }

    public async Task<List<User>> GetByOrganizationIdAsync(Guid organizationId)
        => await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Organization)
            .Include(u => u.Branch)
            .Where(u => u.OrganizationId == organizationId)
            .ToListAsync();

    public async Task<List<User>> GetByBranchIdAsync(Guid branchId)
        => await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Organization)
            .Include(u => u.Branch)
            .Where(u => u.BranchId == branchId)
            .ToListAsync();

    public async Task AddAsync(User user)
        => await _context.Users.AddAsync(user);

    public void Delete(User user)
        => _context.Users.Remove(user);
}
