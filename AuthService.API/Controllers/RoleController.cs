using AuthService.Application.DTOs.Role;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/role?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _roleService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/role/all  — unpaged list for dropdowns/selects
    [HttpGet("all")]
    public async Task<IActionResult> GetAllLookup()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
    {
        var createdRole = await _roleService.CreateRoleAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdRole.Id }, createdRole);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RoleUpdateDto dto)
    {
        var updatedRole = await _roleService.UpdateRoleAsync(id, dto);
        if (updatedRole == null)
            return NotFound();

        return Ok(updatedRole);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
