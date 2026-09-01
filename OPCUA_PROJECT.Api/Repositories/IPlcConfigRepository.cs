using OPCUA_PROJECT.Api.Models;

using OPCUA_PROJECT.Api.DTOs;
namespace OPCUA_PROJECT.Api.Repositories
{
    public interface IPlcConfigRepository
    {
        Task<IEnumerable<PlcConfig>> GetAllAsync();


        Task<PlcConfig?> GetByIdAsync(int id);
    }
}