using AccountingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountingService.Infrastructure.Persistence;

public class AccountingDbContext : DbContext
{
    public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options) { }

    public DbSet<FinancialYear> FinancialYears => Set<FinancialYear>();
    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("acc");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
