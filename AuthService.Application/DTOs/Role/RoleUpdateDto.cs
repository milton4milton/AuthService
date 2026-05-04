namespace AuthService.Application.DTOs.Role;

public class RoleUpdateDto
{
    public string? Name { get; set; }             // optional
    public string? Description { get; set; }      // optional
    public bool? IsActive { get; set; }           // optional
}
