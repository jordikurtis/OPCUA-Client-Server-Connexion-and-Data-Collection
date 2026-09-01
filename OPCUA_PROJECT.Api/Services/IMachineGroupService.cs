using OPCUA_PROJECT.Api.Models;
using OPCUA_PROJECT.Api.DTOs;

namespace OPCUA_PROJECT.Api.Services
{
    public interface IMachineGroupService
    {

        Task<IEnumerable<MachineGroupDto>> GetAllAsync();
    }
}
