namespace AuthService.Application.DTOs.User;

public class UserUpdateDto
{
    public string? UserName { get; set; }        // optional
    public string? Email { get; set; }           // optional
    public string? Password { get; set; }        // optional
    public bool? IsActive { get; set; }          // optional
    public List<Guid>? RoleIds { get; set; }     // optional
}
