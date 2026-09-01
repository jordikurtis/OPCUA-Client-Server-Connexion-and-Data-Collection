using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.Repositories;
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
}
