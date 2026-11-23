using OPCUA_PROJECT.Api.DTOs;

namespace OPCUA_PROJECT.Api.Services
{
    public interface IMachineGroupService
    {
        Task<IEnumerable<MachineGroupDto>> GetAllAsync();

        Task<MachineGroupDto?> GetByIdAsync(int id);

        Task<MachineGroupDto> CreateAsync(MachineGroupCreateDto dto);

        Task<bool> UpdateAsync(int id, MachineGroupUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
