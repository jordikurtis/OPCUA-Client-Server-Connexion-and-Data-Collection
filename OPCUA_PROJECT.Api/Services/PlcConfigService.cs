using OPCUA_PROJECT.Api.DTOs;
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

            return plcs.Select(p =>
                new PlcConfigDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Enabled = p.Enabled
                });
        }

        public async Task<PlcConfigDto?> GetByIdAsync(int id)
        {
            var plc =
                await _repository.GetByIdAsync(id);

            if (plc == null)
                return null;

            return new PlcConfigDto
            {
                Id = plc.Id,
                Name = plc.Name,
                Enabled = plc.Enabled
            };
        }
    }
}
