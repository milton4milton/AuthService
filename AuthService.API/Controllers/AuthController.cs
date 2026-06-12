using AuthService.API.Helpers;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.DTOs.User;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Persistence;
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
    private readonly AuthDbContext _dbContext;

    public AuthController(IUserService userService, JwtSettings jwtSettings, AuthDbContext dbContext)
    {
        _userService = userService;
        _jwtSettings = jwtSettings;
        _dbContext = dbContext;
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

    [HttpGet("db-status")]
    [AllowAnonymous]
    public async Task<IActionResult> DbStatus()
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();
            if (canConnect)
                return Ok(new { status = "Connected" });

            return StatusCode(503, new { status = "Disconnected" });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { status = "Error", message = ex.Message });
        }
    }

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
                UserName        = user.UserName,
                Email           = user.Email,
                Role            = user.Roles.FirstOrDefault(),
                Roles           = user.Roles,
                OrganizationId  = user.OrganizationId,
                OrganizationName = user.OrganizationName,
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
            claims.Add(new Claim(ClaimTypes.Role, role));

        if (user.OrganizationId.HasValue)
            claims.Add(new Claim("organizationId", user.OrganizationId.Value.ToString()));

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
