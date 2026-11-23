using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.Services;

namespace OPCUA_PROJECT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachinesController : ControllerBase
    {
        private readonly IMachineStatusService _service;

        public MachinesController(IMachineStatusService service)
        {
            _service = service;
        }

        // GET api/machines/group/5
        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            var machines = await _service.GetByGroupAsync(groupId);
            return Ok(machines);
        }
    }
}
