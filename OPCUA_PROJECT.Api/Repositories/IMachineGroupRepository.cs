using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public interface IMachineGroupRepository
    {
        Task<IEnumerable<MachineGroup>> GetAllAsync();

        Task<MachineGroup?> GetByIdAsync(int id);

        Task<int> CreateAsync(MachineGroup group);

        Task<bool> UpdateAsync(int id, MachineGroup group);

        Task<bool> DeleteAsync(int id);
    }
}
