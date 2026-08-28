using OPCUA_PROJECT.Api.Models;
namespace OPCUA_PROJECT.Api.Repositories
{
    public class IMachineGroupRepository
    {
        Task<IEnumerable<MachineGroup>> GetAllAsync();

    }
}
