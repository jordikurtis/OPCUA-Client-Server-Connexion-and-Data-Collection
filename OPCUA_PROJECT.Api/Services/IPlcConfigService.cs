using OPCUA_PROJECT.Api.DTOs;

namespace OPCUA_PROJECT.Api.Services
{
    public interface IPlcConfigService
    {
        Task<IEnumerable<PlcConfigDto>> GetAllAsync();

        Task<PlcConfigDto?> GetByIdAsync(int id);
    }
}
