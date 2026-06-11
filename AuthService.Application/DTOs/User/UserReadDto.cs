namespace AuthService.Application.DTOs.User;

public class UserReadDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
}
