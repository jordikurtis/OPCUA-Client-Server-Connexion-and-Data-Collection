using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public interface IMeasurementRepository
    {
        Task<IEnumerable<Measurement>> GetLatestAsync();

        Task<IEnumerable<Measurement>> GetHistoryAsync(int variableId,DateTime from,DateTime to);


    }
}