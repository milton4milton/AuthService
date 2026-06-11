using AuthService.Application.DTOs.User;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/user?page=1&pageSize=10
    [HttpGet]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/user/all  — unpaged list for lookups
    [HttpGet("all")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> GetAllLookup()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    // GET: api/user/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    // GET: api/user/by-organization/{organizationId}
    [HttpGet("by-organization/{organizationId}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> GetByOrganization(Guid organizationId)
    {
        var users = await _userService.GetByOrganizationIdAsync(organizationId);
        return Ok(users);
    }

    // GET: api/user/by-branch/{branchId}
    [HttpGet("by-branch/{branchId}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> GetByBranch(Guid branchId)
    {
        var users = await _userService.GetByBranchIdAsync(branchId);
        return Ok(users);
    }

    // POST: api/user
    [HttpPost]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        var createdUser = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    // PUT: api/user/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
    {
        var updatedUser = await _userService.UpdateUserAsync(id, dto);
        if (updatedUser == null) return NotFound();
        return Ok(updatedUser);
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
