using Npgsql;

namespace OPCUA_PROJECT
{
    public class PostgresService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly MonitoredVariablesService _variables;
        private readonly FaultEventService _faultEvents;
        private readonly LogService? _logger;

        public PostgresService(
            string connectionString,
            MonitoredVariablesService variables,
            FaultEventService faultEvents,
            LogService? logger = null)
        {
            _connectionString = connectionString;
            _variables = variables;
            _faultEvents = faultEvents;
            _logger = logger;
        }

        public async Task SaveAsync(MeasurementsData data)
        {
            try
            {
                // Lookup en mémoire : (PlcName, NodeId) -> VariableId + PlcId
                // + IsFaultIndicator (rempli par MonitoredVariablesService.BuildCacheAsync)
                if (!_variables.TryGetVariableInfo(data.PlcName, data.NodeId, out var info))
                {
                    if (_logger != null)
                        await _logger.WarningAsync($"Variable inconnue en DB (NodeId = {data.NodeId}), écriture ignorée.", data.PlcName);
                    else
                        Console.WriteLine($"DB: NodeId {data.NodeId} inconnu, écriture ignorée.");
                    return;
                }

                const string sql = @"INSERT INTO measurements (variable_id, value, status, source_timestamp)
                                      VALUES (@variableId, @value, @status, @ts);";

                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@variableId", info.VariableId);
                cmd.Parameters.AddWithValue("@value", data.Value?.ToString() ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@status", data.Status);
                cmd.Parameters.AddWithValue("@ts", data.SourceTimestamp);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"DB Ok -> {data}");

                // Fehlermeldung : measurements garde l'historique brut comme
                // pour toute variable ; en plus, si celle-ci est marquée
                // comme indicateur de défaut, on fait passer la valeur par
                // la détection de transition (ouverture/fermeture fault_events).
                if (info.IsFaultIndicator)
                {
                    await _faultEvents.HandleValueAsync(
                        info.PlcId, info.VariableId, data.Value?.ToString(), data.SourceTimestamp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Erreur : {ex.Message}");
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                Console.WriteLine("DB Connexion PostgreSQL OK");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Connexion impossible: {ex.Message}");
                return false;
            }
        }
    }
}
