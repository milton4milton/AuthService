using AccountingService.Domain.Interfaces;
using AccountingService.Infrastructure.Persistence;

namespace AccountingService.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountingDbContext _context;

    public UnitOfWork(AccountingDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
