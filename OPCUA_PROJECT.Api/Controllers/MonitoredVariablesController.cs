using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.Models;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoredVariablesController: ControllerBase
    {
        private readonly IMonitoredVariableRepository _repository;

        public MonitoredVariablesController(
            IMonitoredVariableRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var variables =
                await _repository.GetAllAsync();

            return Ok(variables);
        }

        
    }
}
