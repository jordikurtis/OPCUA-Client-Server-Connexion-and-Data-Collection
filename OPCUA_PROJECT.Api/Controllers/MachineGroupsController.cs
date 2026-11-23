using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.DTOs;
using OPCUA_PROJECT.Api.Services;

namespace OPCUA_PROJECT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineGroupsController : ControllerBase
{
    private readonly IMachineGroupService _service;

    public MachineGroupsController(IMachineGroupService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _service.GetAllAsync();
        return Ok(groups);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var group = await _service.GetByIdAsync(id);
        if (group == null)
            return NotFound();

        return Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MachineGroupCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name ist erforderlich.");

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MachineGroupUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name ist erforderlich.");

        var updated = await _service.UpdateAsync(id, dto);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Grâce à la migration 002 (ON DELETE SET NULL), les PLCs rattachés
        // à ce groupe deviennent automatiquement "sans groupe" — pas
        // d'exception FK à gérer ici, contrairement à PlcConfigsController.
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
