using OPCUA_PROJECT.Api.DTOs;
using OPCUA_PROJECT.Api.Models;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Services
{
    public class MachineGroupService : IMachineGroupService
    {
        private readonly IMachineGroupRepository _repository;

        public MachineGroupService(IMachineGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MachineGroupDto>> GetAllAsync()
        {
            var groups =
                await _repository.GetAllAsync();

            return groups.Select(ToDto);
        }

        public async Task<MachineGroupDto?> GetByIdAsync(int id)
        {
            var group = await _repository.GetByIdAsync(id);
            return group == null ? null : ToDto(group);
        }

        public async Task<MachineGroupDto> CreateAsync(MachineGroupCreateDto dto)
        {
            var group = new MachineGroup
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var newId = await _repository.CreateAsync(group);
            group.Id = newId;

            return ToDto(group);
        }

        public async Task<bool> UpdateAsync(int id, MachineGroupUpdateDto dto)
        {
            var group = new MachineGroup
            {
                Name = dto.Name,
                Description = dto.Description
            };

            return await _repository.UpdateAsync(id, group);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static MachineGroupDto ToDto(MachineGroup g) => new()
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description
        };
    }
}
