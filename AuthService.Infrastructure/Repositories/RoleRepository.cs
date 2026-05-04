using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
        => await _context.Roles.FindAsync(id);

    public async Task<IEnumerable<Role>> GetAllAsync()
        => await _context.Roles.ToListAsync();

    public void Delete(Role role)
    {
        _context.Roles.Remove(role);
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }
}
