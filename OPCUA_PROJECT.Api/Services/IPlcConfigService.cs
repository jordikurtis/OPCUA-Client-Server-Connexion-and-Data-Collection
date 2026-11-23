using OPCUA_PROJECT.Api.DTOs;

namespace OPCUA_PROJECT.Api.Services
{
    public interface IPlcConfigService
    {
        Task<IEnumerable<PlcConfigDto>> GetAllAsync();

        Task<PlcConfigDto?> GetByIdAsync(int id);

        Task<PlcConfigDto> CreateAsync(PlcConfigCreateDto dto);

        Task<bool> UpdateAsync(int id, PlcConfigUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
