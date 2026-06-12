using AccountingService.Application.DTOs.ChartOfAccount;
using AccountingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountingService.API.Controllers;

[ApiController]
[Route("api/chart-of-accounts")]
[Authorize]
public class ChartOfAccountController : ControllerBase
{
    private readonly IChartOfAccountService _service;

    public ChartOfAccountController(IChartOfAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] Guid organizationId,
        [FromQuery] Guid? parentId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetPagedAsync(organizationId, parentId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] Guid organizationId)
    {
        var result = await _service.GetAllByOrganizationAsync(organizationId);
        return Ok(result);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree([FromQuery] Guid organizationId)
    {
        var result = await _service.GetTreeAsync(organizationId);
        return Ok(result);
    }

    [HttpGet("children")]
    public async Task<IActionResult> GetChildren(
        [FromQuery] Guid organizationId,
        [FromQuery] Guid? parentId)
    {
        var result = await _service.GetChildrenAsync(organizationId, parentId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ChartOfAccountCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ChartOfAccountUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
