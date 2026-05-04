using AuthService.Application.DTOs.Role;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // JWT required
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/Role
    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("byid")]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }



    //// GET: api/Role/{id}
    //[HttpGet("{id}")]
    ////[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> GetById(Guid id)
    //{
    //    var role = await _roleService.GetByIdAsync(id);
    //    if (role == null)
    //        return NotFound();

    //    return Ok(role);
    //}

    // POST: api/Role
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
    {
        var createdRole = await _roleService.CreateRoleAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdRole.Id }, createdRole);
    }

    // PUT: api/Role/{id}
    [HttpPut("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RoleUpdateDto dto)
    {
        var updatedRole = await _roleService.UpdateRoleAsync(id, dto);
        if (updatedRole == null)
            return NotFound();

        return Ok(updatedRole);
    }

    // DELETE: api/Role/{id}
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
