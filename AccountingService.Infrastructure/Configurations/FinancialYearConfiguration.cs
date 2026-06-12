using AccountingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingService.Infrastructure.Configurations;

public class FinancialYearConfiguration : IEntityTypeConfiguration<FinancialYear>
{
    public void Configure(EntityTypeBuilder<FinancialYear> builder)
    {
        builder.ToTable("FinancialYears");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.OrganizationId).IsRequired();
        builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
        builder.Property(f => f.StartDate).IsRequired();
        builder.Property(f => f.EndDate).IsRequired();
        builder.Property(f => f.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(f => f.IsClosed).IsRequired().HasDefaultValue(false);
        builder.Property(f => f.CreatedAt).IsRequired();
        builder.Property(f => f.UpdatedAt).IsRequired(false);

        builder.HasIndex(f => new { f.OrganizationId, f.Name }).IsUnique();
    }
}
