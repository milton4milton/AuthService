using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class UiAccessPermissionConfiguration : IEntityTypeConfiguration<UiAccessPermission>
{
    public void Configure(EntityTypeBuilder<UiAccessPermission> builder)
    {
        builder.ToTable("UiAccessPermissions");
        builder.HasKey(x => x.Id);
    }
}
