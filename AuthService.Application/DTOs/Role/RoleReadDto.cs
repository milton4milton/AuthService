namespace AuthService.Application.DTOs.Role;

public class RoleCreateDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
