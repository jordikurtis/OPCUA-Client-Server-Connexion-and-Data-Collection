
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;

namespace OPCUA_PROJECT
{
    /// <summary>
    /// Remplace ConfigServiceJSON.
    /// Charge la configuration des PLCs directement depuis PostgreSQL.
    /// Plus de fichier JSON dans le code.
    /// </summary>
    public class PostgresConfigService
    {
        private readonly string _connectionString;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PostgresConfigService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // CHARGEMENT DES PLCs DEPUIS LA DB
        // Retourne uniquement les PLCs actifs (enabled = true)
        public async Task<List<PlcDefinition>> LoadPlcsAsync()
        {
            var plcs = new List<PlcDefinition>();

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                // On lit uniquement les PLCs actifs
                await using var cmd = new NpgsqlCommand(@"
                    SELECT name, endpoint_url, enabled, config_json
                    FROM plc_configs
                    WHERE enabled = true
                    ORDER BY id", conn);

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var name = reader.GetString(0);
                    var endpointUrl = reader.GetString(1);
                    var enabled = reader.GetBoolean(2);
                    var configJson = reader.GetString(3);

                    // Désérialiser la colonne config_json
                    // → reconstruit les Nodes + Variables en mémoire
                    var configPart = JsonSerializer.Deserialize<PlcConfigJson>(
                                        configJson, JsonOptions)
                                     ?? new PlcConfigJson();

                    plcs.Add(new PlcDefinition
                    {
                        Name = name,
                        EndpointUrl = endpointUrl,
                        Enabled = enabled,
                        Nodes = configPart.Nodes
                    });

                    Console.WriteLine($"[Config] → {name} ({configPart.Nodes.Count} node(s))");
                }

                Console.WriteLine($"[Config] {plcs.Count} PLC(s) chargé(s) depuis PostgreSQL.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Config] Erreur chargement config : {ex.Message}");
            }

            return plcs;
        }
    }
    // MODÈLE INTERNE
    // Utilisé uniquement pour désérialiser la colonne config_json
    // N'apparaît pas ailleurs dans le code
    public class PlcConfigJson
    {
        public List<NodeDefinition> Nodes { get; set; } = new();
    }
}
