using AuthService.API.Helpers;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.DTOs.User;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IUserService userService, JwtSettings jwtSettings)
    {
        _userService = userService;
        _jwtSettings = jwtSettings;
    }

    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    //{
    //    var user = await _userService.ValidateUserAsync(dto.Email, dto.Password);
    //    if (user == null)
    //        return Unauthorized(new { message = "Invalid credentials" });

    //    var token = GenerateJwtToken(user);
    //    return Ok(new { token });
    //}

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _userService.ValidateUserAsync(dto.Email, dto.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = GenerateJwtToken(user);

        var response = new AuthResponseDto
        {
            Token = token,
            User = new AuthUserDto
            {
                UserName = user.UserName,
                Role = user.Roles.FirstOrDefault() // ধরছি Admin
            }
        };

        return Ok(response);
    }


    private string GenerateJwtToken(UserReadDto user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
