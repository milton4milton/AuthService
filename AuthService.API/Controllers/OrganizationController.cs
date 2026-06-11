using AuthService.Application.DTOs.Organization;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _orgService;

    public OrganizationController(IOrganizationService orgService)
    {
        _orgService = orgService;
    }

    // GET: api/organization?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _orgService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/organization/all  — unpaged list for dropdowns/selects
    [HttpGet("all")]
    public async Task<IActionResult> GetAllLookup()
    {
        var orgs = await _orgService.GetAllAsync();
        return Ok(orgs);
    }

    // GET: api/organization/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var org = await _orgService.GetByIdAsync(id);
        if (org == null) return NotFound();
        return Ok(org);
    }

    // POST: api/organization
    [HttpPost]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Create([FromBody] OrganizationCreateDto dto)
    {
        var created = await _orgService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/organization/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] OrganizationUpdateDto dto)
    {
        var updated = await _orgService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    // DELETE: api/organization/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _orgService.DeleteAsync(id);
        return NoContent();
    }
}
