using AuthService.Application.DTOs.User;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // JWT secured
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/User
    [HttpGet]
    [Authorize(Roles = "Admin")] // Only Admin can see all users
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    // GET: api/User/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    // POST: api/User
    [HttpPost]
    [Authorize(Roles = "Admin")] // Only Admin can create users
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        var createdUser = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    // PUT: api/User/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Only Admin can update users
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
    {
        var updatedUser = await _userService.UpdateUserAsync(id, dto);
        if (updatedUser == null) return NotFound();
        return Ok(updatedUser);
    }

    // DELETE: api/User/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only Admin can delete users
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        await _userService.DeleteUserAsync(id); // Add this method in IUserService & UserService
        return NoContent();
    }
}
