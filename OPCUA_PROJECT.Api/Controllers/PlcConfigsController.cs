using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.Repositories;
using OPCUA_PROJECT.Api.Services;
namespace OPCUA_PROJECT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlcConfigsController : ControllerBase
    {
        private readonly IPlcConfigService _service;


        private readonly IMonitoredVariableRepository _variableRepository;

        public PlcConfigsController(IPlcConfigService service,IMonitoredVariableRepository variableRepository){
            _service = service;
            _variableRepository = variableRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plcs =
                await _service.GetAllAsync();

            return Ok(plcs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plc =
                await _service.GetByIdAsync(id);

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

            var variables =
                await _variableRepository.GetByPlcNameAsync(plc.Name);

            return Ok(variables);
        }
    }
}
