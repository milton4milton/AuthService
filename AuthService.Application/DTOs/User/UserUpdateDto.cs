namespace AuthService.Application.DTOs.User;

public class UserUpdateDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? RoleIds { get; set; }
    public Guid? OrganizationId { get; set; }
    public List<Guid>? BranchIds { get; set; }
}
