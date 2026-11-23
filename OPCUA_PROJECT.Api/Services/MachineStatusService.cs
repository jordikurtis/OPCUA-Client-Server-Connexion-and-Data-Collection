using OPCUA_PROJECT.Api.DTOs;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Services
{
    public class MachineStatusService : IMachineStatusService
    {
        private readonly IMachineStatusRepository _repository;

        public MachineStatusService(IMachineStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MachineStatusDto>> GetByGroupAsync(int groupId)
        {
            var machines = await _repository.GetByGroupAsync(groupId);

            return machines.Select(m => new MachineStatusDto
            {
                PlcName = m.PlcName,
                LastSeen = m.LastSeen,
                IsOnline = m.IsOnline
            });
        }
    }
}
