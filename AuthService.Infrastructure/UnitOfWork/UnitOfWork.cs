using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;

namespace AuthService.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;

    public UnitOfWork(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
