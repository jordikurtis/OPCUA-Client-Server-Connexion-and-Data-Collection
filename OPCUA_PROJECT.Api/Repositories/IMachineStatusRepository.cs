using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public interface IMachineStatusRepository
    {
        Task<IEnumerable<MachineStatus>> GetByGroupAsync(int groupId);
    }
}
