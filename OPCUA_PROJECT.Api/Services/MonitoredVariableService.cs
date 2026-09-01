using OPCUA_PROJECT.Api.DTOs;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Services
{
    public class MonitoredVariableService : IMonitoredVariableService
    {
        private readonly IMonitoredVariableRepository  _repository;

        public MonitoredVariableService(
            IMonitoredVariableRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MonitoredVariableDto>>
            GetAllAsync()
        {
            var variables =
                await _repository.GetAllAsync();

            return variables.Select(v => new MonitoredVariableDto
                {
                    Id = v.Id,
                    VariableName = v.VariableName,
                    NodeName = v.NodeName,
                    Enabled = v.Enabled
                });
        }
    }
}
