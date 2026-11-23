using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace OPCUA_PROJECT
{
    /// <summary>
    /// Charge la configuration des PLCs depuis PostgreSQL.
    /// Les Nodes/Variables viennent de monitored_variables, filtrées par
    /// plc_id, y compris le flag is_fault_indicator (Fehlermeldung).
    /// </summary>
    public class PostgresConfigService
    {
        private readonly string _connectionString;

        public PostgresConfigService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<PlcDefinition>> LoadPlcsAsync()
        {
            var plcs = new List<PlcDefinition>();

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var plcRows = new List<(int Id, string Name, string EndpointUrl)>();

                await using (var plcCmd = new NpgsqlCommand(@"
                    SELECT id, name, endpoint_url
                    FROM plc_configs
                    WHERE enabled = true
                    ORDER BY id", conn))
                await using (var reader = await plcCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        plcRows.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                    }
                }

                foreach (var (id, name, endpointUrl) in plcRows)
                {
                    var nodes = await LoadNodesForPlcAsync(conn, id);

                    plcs.Add(new PlcDefinition
                    {
                        Id = id,
                        Name = name,
                        EndpointUrl = endpointUrl,
                        Enabled = true,
                        Nodes = nodes
                    });

                    var variableCount = nodes.Sum(n => n.Variables.Count);
                    Console.WriteLine($"[Config] → {name} ({nodes.Count} node(s), {variableCount} variable(s))");
                }

                Console.WriteLine($"[Config] {plcs.Count} PLC(s) chargé(s) depuis PostgreSQL.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Config] Erreur chargement config : {ex.Message}");
            }

            return plcs;
        }

        private static async Task<List<NodeDefinition>> LoadNodesForPlcAsync(
            NpgsqlConnection conn, int plcId)
        {
            await using var cmd = new NpgsqlCommand(@"
                SELECT node_name, variable_name, node_id, data_type, is_fault_indicator
                FROM monitored_variables
                WHERE plc_id = @plcId AND enabled = true
                ORDER BY node_name, variable_name", conn);

            cmd.Parameters.AddWithValue("@plcId", plcId);

            var nodesByName = new Dictionary<string, NodeDefinition>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var nodeName = reader.GetString(0);
                var variableName = reader.GetString(1);
                var nodeId = reader.GetString(2);
                var dataType = reader.GetString(3);
                var isFaultIndicator = reader.GetBoolean(4);

                if (!nodesByName.TryGetValue(nodeName, out var node))
                {
                    node = new NodeDefinition { Name = nodeName };
                    nodesByName[nodeName] = node;
                }

                node.Variables.Add(new VariableDefinition
                {
                    Name = variableName,
                    NodeId = nodeId,
                    DataType = dataType,
                    Enabled = true,
                    IsFaultIndicator = isFaultIndicator
                });
            }

            return nodesByName.Values.ToList();
        }
    }
}
