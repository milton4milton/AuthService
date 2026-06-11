namespace AuthService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; }

    public Guid? OrganizationId { get; set; }
    public Guid? BranchId { get; set; }

    public Organization? Organization { get; set; }
    public Branch? Branch { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
