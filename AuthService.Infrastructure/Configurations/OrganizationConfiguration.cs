using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(150);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasMany(x => x.Branches)
               .WithOne(x => x.Organization)
               .HasForeignKey(x => x.OrganizationId)
               .OnDelete(DeleteBehavior.Cascade);

        // Restrict prevents deleting an org that still has users assigned to it.
        // SQL Server disallows SetNull here because Org→Branches→Users creates a second cascade path.
        builder.HasMany(x => x.Users)
               .WithOne(x => x.Organization)
               .HasForeignKey(x => x.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
