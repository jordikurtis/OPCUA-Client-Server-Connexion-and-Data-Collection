using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementsController
        : ControllerBase
    {
        private readonly IMeasurementRepository _repository;

        public MeasurementsController(
            IMeasurementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var data =
                await _repository.GetLatestAsync();

            return Ok(data);
        }
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(int variableId,DateTime from,DateTime to)
        {
            var history =
                await _repository.GetHistoryAsync(variableId,from,to);

            return Ok(history);
        }
    }
}
