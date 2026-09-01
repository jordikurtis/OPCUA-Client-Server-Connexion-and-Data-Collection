using OPCUA_PROJECT.Api.Models;
using OPCUA_PROJECT.Api.Repositories;
using OPCUA_PROJECT.Api.DTOs;

namespace OPCUA_PROJECT.Api.Services
{
    public class MachineGroupService : IMachineGroupService
    {
        private readonly IMachineGroupRepository _repository;

        public MachineGroupService( IMachineGroupRepository repository)
        {
            _repository = repository;
        }

      public  async Task<IEnumerable<MachineGroupDto>> GetAllAsync()
        {
            var groups = 
                await _repository.GetAllAsync();
           // return await _repository.GetAllAsync();

            return groups.Select (g => new MachineGroupDto {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description 
            });
        }
    }
}
