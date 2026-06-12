using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class UserBranchConfiguration : IEntityTypeConfiguration<UserBranch>
{
    public void Configure(EntityTypeBuilder<UserBranch> builder)
    {
        builder.ToTable("UserBranches");
        builder.HasKey(x => new { x.UserId, x.BranchId });

        builder.HasOne(x => x.User)
               .WithMany(x => x.UserBranches)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Branch)
               .WithMany(x => x.UserBranches)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
