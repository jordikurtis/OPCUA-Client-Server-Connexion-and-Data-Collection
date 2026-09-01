using OPCUA_PROJECT.Api.DTOs;

namespace OPCUA_PROJECT.Api.Services
{
    public interface IMeasurementService
    {
        Task<IEnumerable<MeasurementDto>>GetLatestAsync();

        Task<IEnumerable<MeasurementDto>>GetHistoryAsync(int variableId,DateTime from,DateTime to);
    }
}
