using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public interface IPlcConfigRepository
    {
        Task<IEnumerable<PlcConfig>> GetAllAsync();

        Task<PlcConfig?> GetByIdAsync(int id);

        Task<int> CreateAsync(PlcConfig plc);

        Task<bool> UpdateAsync(int id, PlcConfig plc);

        Task<bool> DeleteAsync(int id);
    }
}
