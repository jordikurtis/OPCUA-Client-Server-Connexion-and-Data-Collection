using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OPCUA_PROJECT
{
    /// <summary>
    /// Construit un cache en mémoire (plc_name + NodeId -> infos variable)
    /// à partir de monitored_variables, pour que PostgresService.SaveAsync
    /// puisse retrouver rapidement le variable_id (+ plc_id, + si c'est un
    /// indicateur de défaut) de chaque mesure reçue, sans requête DB
    /// supplémentaire à chaque écriture.
    ///
    /// Ce service ne fait que LIRE — monitored_variables est la source de
    /// vérité, remplie via l'Admin Panel/API.
    /// </summary>
    public class MonitoredVariablesService
    {
        private readonly string _connectionString;

        public readonly struct VariableInfo
        {
            public int VariableId { get; init; }
            public int PlcId { get; init; }
            public bool IsFaultIndicator { get; init; }
        }

        private readonly Dictionary<(string PlcName, string NodeId), VariableInfo> _cache = new();

        public MonitoredVariablesService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task BuildCacheAsync(List<PlcDefinition> plcs, LogService? logger = null)
        {
            _cache.Clear();

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT id, node_id, is_fault_indicator
                FROM monitored_variables
                WHERE plc_id = @plcId AND enabled = true";

            foreach (var plc in plcs)
            {
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@plcId", plc.Id);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var variableId = reader.GetInt32(0);
                    var nodeId = reader.GetString(1);
                    var isFaultIndicator = reader.GetBoolean(2);

                    _cache[(plc.Name, nodeId)] = new VariableInfo
                    {
                        VariableId = variableId,
                        PlcId = plc.Id,
                        IsFaultIndicator = isFaultIndicator
                    };
                }
            }

            if (logger != null)
                await logger.InfoAsync($"[MonitoredVariablesService] Cache construit : {_cache.Count} variable(s).");
        }

        public bool TryGetVariableInfo(string plcName, string nodeId, out VariableInfo info)
            => _cache.TryGetValue((plcName, nodeId), out info);
    }
}
