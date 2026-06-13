using AuthService.Application.DTOs.User;

namespace AuthService.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }
    public AuthUserDto User { get; set; }
}

public class AuthUserDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }          // primary role (first assigned)
    public List<string> Roles { get; set; } = new();
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public List<BranchInfoDto> Branches { get; set; } = new();
}
