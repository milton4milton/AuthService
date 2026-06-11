using AuthService.Application.DTOs.Branch;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BranchController : ControllerBase
{
    private readonly IBranchService _branchService;

    public BranchController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    // GET: api/branch?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _branchService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    // GET: api/branch/all  — unpaged list for dropdowns/selects
    [HttpGet("all")]
    public async Task<IActionResult> GetAllLookup()
    {
        var branches = await _branchService.GetAllAsync();
        return Ok(branches);
    }

    // GET: api/branch/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var branch = await _branchService.GetByIdAsync(id);
        if (branch == null) return NotFound();
        return Ok(branch);
    }

    // GET: api/branch/by-organization/{organizationId}
    [HttpGet("by-organization/{organizationId}")]
    public async Task<IActionResult> GetByOrganization(Guid organizationId)
    {
        var branches = await _branchService.GetByOrganizationIdAsync(organizationId);
        return Ok(branches);
    }

    // POST: api/branch
    [HttpPost]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Create([FromBody] BranchCreateDto dto)
    {
        var created = await _branchService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/branch/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BranchUpdateDto dto)
    {
        var updated = await _branchService.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    // DELETE: api/branch/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _branchService.DeleteAsync(id);
        return NoContent();
    }
}
