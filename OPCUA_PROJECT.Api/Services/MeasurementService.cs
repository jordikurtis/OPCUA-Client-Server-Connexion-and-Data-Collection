using OPCUA_PROJECT.Api.DTOs;
using OPCUA_PROJECT.Api.Repositories;

namespace OPCUA_PROJECT.Api.Services
{
    public class MeasurementService
        : IMeasurementService
    {
        private readonly IMeasurementRepository
            _repository;

        public MeasurementService(
            IMeasurementRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MeasurementDto>>
            GetLatestAsync()
        {
            var data =
                await _repository.GetLatestAsync();

            return data.Select(m =>
                new MeasurementDto
                {
                    VariableId = m.VariableId,
                    Value = m.Value,
                    Status = m.Status,
                    Timestamp = m.SourceTimestamp
                });
        }

        public async Task<IEnumerable<MeasurementDto>>
            GetHistoryAsync(
                int variableId,
                DateTime from,
                DateTime to)
        {
            var data =
                await _repository.GetHistoryAsync(
                    variableId,
                    from,
                    to);

            return data.Select(m =>
                new MeasurementDto
                {
                    VariableId = m.VariableId,
                    Value = m.Value,
                    Status = m.Status,
                    Timestamp = m.SourceTimestamp
                });
        }
    }
}
