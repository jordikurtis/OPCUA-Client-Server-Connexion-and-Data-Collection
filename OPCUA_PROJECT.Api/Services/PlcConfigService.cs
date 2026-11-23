using OPCUA_PROJECT.Api.DTOs;
using OPCUA_PROJECT.Api.Models;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Services
{
    public class PlcConfigService : IPlcConfigService
    {
        private readonly IPlcConfigRepository _repository;

        public PlcConfigService(
            IPlcConfigRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PlcConfigDto>>
            GetAllAsync()
        {
            var plcs =
                await _repository.GetAllAsync();

            return plcs.Select(ToDto);
        }

        public async Task<PlcConfigDto?> GetByIdAsync(int id)
        {
            var plc =
                await _repository.GetByIdAsync(id);

            return plc == null ? null : ToDto(plc);
        }

        public async Task<PlcConfigDto> CreateAsync(PlcConfigCreateDto dto)
        {
            var plc = new PlcConfig
            {
                Name = dto.Name,
                EndpointUrl = dto.EndpointUrl,
                Enabled = dto.Enabled,
                GroupId = dto.GroupId
            };

            var newId = await _repository.CreateAsync(plc);
            plc.Id = newId;

            return ToDto(plc);
        }

        public async Task<bool> UpdateAsync(int id, PlcConfigUpdateDto dto)
        {
            var plc = new PlcConfig
            {
                Name = dto.Name,
                EndpointUrl = dto.EndpointUrl,
                Enabled = dto.Enabled,
                GroupId = dto.GroupId
            };

            return await _repository.UpdateAsync(id, plc);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        // Mapping Model -> DTO centralisé : EndpointUrl volontairement absent
        private static PlcConfigDto ToDto(PlcConfig plc) => new()
        {
            Id = plc.Id,
            Name = plc.Name,
            Enabled = plc.Enabled,
            GroupId = plc.GroupId
        };
    }
}
