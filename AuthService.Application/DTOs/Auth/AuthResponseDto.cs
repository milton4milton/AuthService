namespace AuthService.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }
    public AuthUserDto User { get; set; }
}

public class AuthUserDto
{
    public string UserName { get; set; }
    public string Role { get; set; } // single role (Admin)
}
