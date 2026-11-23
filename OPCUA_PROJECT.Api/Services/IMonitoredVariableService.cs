using OPCUA_PROJECT.Api.DTOs;

namespace OPCUA_PROJECT.Api.Services
{
    public interface IMonitoredVariableService
    {
        Task<IEnumerable<MonitoredVariableDto>> GetAllAsync();

        Task<IEnumerable<MonitoredVariableDto>> GetByPlcNameAsync(string plcName);
    }
}
