using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineGroupsController : ControllerBase
{
    private readonly IMachineGroupRepository _repository;

    public MachineGroupsController(
        IMachineGroupRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups =
            await _repository.GetAllAsync();

        return Ok(groups);
    }
}
