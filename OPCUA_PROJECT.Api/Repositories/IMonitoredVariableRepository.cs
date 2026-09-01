using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public interface IMonitoredVariableRepository
    {
        Task<IEnumerable<MonitoredVariable>> GetAllAsync();

        Task<IEnumerable<MonitoredVariable>>GetByPlcNameAsync(string plcName);
    }
}
