using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OPCUA_PROJECT.Api.DTOs;
using OPCUA_PROJECT.Api.Services;

namespace OPCUA_PROJECT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlcConfigsController : ControllerBase
    {
        private readonly IPlcConfigService _service;
        private readonly IMonitoredVariableService _variableService;

        public PlcConfigsController(
            IPlcConfigService service,
            IMonitoredVariableService variableService)
        {
            _service = service;
            _variableService = variableService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plcs = await _service.GetAllAsync();
            return Ok(plcs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plc = await _service.GetByIdAsync(id);
            if (plc == null)
                return NotFound();

            return Ok(plc);
        }

        [HttpGet("{id}/Variables")]
        public async Task<IActionResult> GetVariables(int id)
        {
            var plc = await _service.GetByIdAsync(id);
            if (plc == null)
                return NotFound();

            var variables = await _variableService.GetByPlcNameAsync(plc.Name);
            return Ok(variables);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlcConfigCreateDto dto)
        {
            // Création : EndpointUrl obligatoire (impossible de connecter un PLC sans URL)
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.EndpointUrl))
                return BadRequest("Name und EndpointUrl sind erforderlich.");

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlcConfigUpdateDto dto)
        {
            // Édition : EndpointUrl optionnel — vide = on garde l'ancienne valeur
            // (voir PlcConfigRepository.UpdateAsync). Seul Name reste obligatoire.
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
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                return Conflict("Dieser PLC kann nicht gelöscht werden, da noch Daten darauf verweisen.");
            }
        }
    }
}
