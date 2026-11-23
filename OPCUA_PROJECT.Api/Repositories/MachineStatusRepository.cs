using Dapper;
using Npgsql;
using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public class MachineStatusRepository : IMachineStatusRepository
    {
        private readonly string _connectionString;

        // Seuil au-delà duquel une machine est considérée Offline
        // (pas de mesure reçue depuis ce délai). En dur pour l'instant.
        private const int OnlineThresholdSeconds = 30;

        public MachineStatusRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<MachineStatus>> GetByGroupAsync(int groupId)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    pc.name AS PlcName,
                    pc.endpoint_url AS EndpointUrl,
                    MAX(m.source_timestamp) AS LastSeen,
                    CASE
                        WHEN MAX(m.source_timestamp) > now() - (@thresholdSeconds || ' seconds')::interval
                        THEN true
                        ELSE false
                    END AS IsOnline
                FROM plc_configs pc
                LEFT JOIN monitored_variables mv ON mv.plc_name = pc.name
                LEFT JOIN measurements m ON m.variable_id = mv.id
                WHERE pc.group_id = @groupId
                  AND pc.enabled = true
                GROUP BY pc.name, pc.endpoint_url
                ORDER BY pc.name";

            return await connection.QueryAsync<MachineStatus>(sql, new
            {
                groupId,
                thresholdSeconds = OnlineThresholdSeconds
            });
        }
    }
}
