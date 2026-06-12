namespace AuthService.Application.DTOs.User;

public class UserCreateDto
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<Guid> RoleIds { get; set; } = new();
    public Guid? OrganizationId { get; set; }
    public List<Guid> BranchIds { get; set; } = new();
}
