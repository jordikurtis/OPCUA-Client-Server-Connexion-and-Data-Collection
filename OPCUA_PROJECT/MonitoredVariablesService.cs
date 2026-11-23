using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUA_PROJECT
{
    // <summary>
    /// Synchronise la table monitored_variables à partir des * PlcDefinition *
    /// déjà chargées via PostgresConfigService (donc depuis plc_configs).
    /// Pas de double saisie : tu configures toujours tes PLCs/nodes/variables
    /// au même endroit qu'aujourd'hui (plc_configs.config_json). 
    /// ---> Cette classe se contente de "mettre à plat" cette config dans monitored_variables,
    /// pour que  " measurements " puisse pointer vers un variable_id léger.
    /// Fournit aussi un cache en mémoire (NodeId -> variable_id) pour éviter
    /// une requête DB à chaque mesure reçue (potentiellement haute fréquence).
    /// </summary>
    public class MonitoredVariablesService
    {

        private readonly string _connectionString;

        // Cache NodeId (ex: "ns=3;i=1001") -> variable_id
        // Rempli au démarrage par SyncAsync(), relu par PostgresService.
        private readonly Dictionary<string, int> _variableIdCache = new();

        public MonitoredVariablesService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// À appeler une fois au démarrage, juste après avoir chargé
        /// les PlcDefinition depuis PostgresConfigService.LoadPlcsAsync().
        /// Upsert chaque variable active dans monitored_variables,
        /// et remplit le cache en mémoire.
        /// </summary>
        public async Task SyncAsync(List<PlcDefinition> plcs, LogService? logger = null)
        {
            _variableIdCache.Clear();

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            foreach (var plc in plcs)
            {
                // Résoudre le group_id de ce PLC (via plc_configs.group_id) une seule fois
                int? groupId = await GetGroupIdForPlcAsync(conn, plc.Name);

                foreach (var node in plc.Nodes)
                {
                    foreach (var variable in node.Variables)
                    {
                        if (!variable.Enabled)
                            continue;

                        var variableId = await UpsertVariableAsync(
                            conn, plc.Name, groupId, node.Name, variable.Name,
                            variable.NodeId, variable.DataType);

                        _variableIdCache[variable.NodeId] = variableId;
                    }
                }
            }

            if (logger != null)
                await logger.InfoAsync(
                    $"[MonitoredVariablesService] {_variableIdCache.Count} variable(s) synchronisée(s).");
        }

        /// <summary>
        /// Lookup rapide (en mémoire) : NodeId OPC UA -> variable_id SQL.
        /// Utilisé par PostgresService.SaveAsync avant chaque INSERT dans measurements.
        /// </summary>
        public bool TryGetVariableId(string nodeId, out int variableId)
            => _variableIdCache.TryGetValue(nodeId, out variableId);

        private static async Task<int?> GetGroupIdForPlcAsync(NpgsqlConnection conn, string plcName)
        {
            await using var cmd = new NpgsqlCommand(
                "SELECT group_id FROM plc_configs WHERE name = @name", conn);
            cmd.Parameters.AddWithValue("@name", plcName);

            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }

        private static async Task<int> UpsertVariableAsync(
            NpgsqlConnection conn, string plcName, int? groupId,
            string nodeName, string variableName, string nodeId, string dataType)
        {
            // ON CONFLICT (node_id) : le NodeId OPC UA est la clé stable.
            // Si la variable existe déjà, on met juste à jour ses métadonnées
            // (au cas où le node/groupe aurait changé) et on récupère son id.
            const string sql = @"
                INSERT INTO monitored_variables
                    (plc_name, group_id, node_name, variable_name, node_id, data_type, enabled)
                VALUES
                    (@plcName, @groupId, @nodeName, @variableName, @nodeId, @dataType, true)
                ON CONFLICT (node_id) DO UPDATE SET
                    plc_name      = EXCLUDED.plc_name,
                    group_id      = EXCLUDED.group_id,
                    node_name     = EXCLUDED.node_name,
                    variable_name = EXCLUDED.variable_name,
                    data_type     = EXCLUDED.data_type,
                    enabled       = true
                RETURNING id;";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@plcName", plcName);
            cmd.Parameters.AddWithValue("@groupId", (object?)groupId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nodeName", nodeName);
            cmd.Parameters.AddWithValue("@variableName", variableName);
            cmd.Parameters.AddWithValue("@nodeId", nodeId);
            cmd.Parameters.AddWithValue("@dataType", (object?)dataType ?? DBNull.Value);

            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }
    }
}

