using Microsoft.AspNetCore.Mvc;
using OPCUA_PROJECT.Api.Services;

namespace OPCUA_PROJECT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementsController: ControllerBase
    {
        private readonly IMeasurementService _service;
        /*
                public MeasurementsController (IMeasurementRepository repository)
                {
                    _repository = repository;
                }
                */
        public MeasurementsController(IMeasurementService service)
        {
            _service = service;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var data =
                await _service.GetLatestAsync();

            return Ok(data);
        }
        [HttpGet("history")]
       /* public async Task<IActionResult> GetHistory(int variableId,DateTime from,DateTime to)
        {
            var history =
                await _service.GetHistoryAsync(variableId,from,to);

            return Ok(history);
        }
        */
       public async Task<IActionResult> GetHistory(int variableId, DateTime from, DateTime to)
        {
            var history = 
                await _service.GetHistoryAsync(variableId, from, to);
            return Ok(history);
        }
    }
}
