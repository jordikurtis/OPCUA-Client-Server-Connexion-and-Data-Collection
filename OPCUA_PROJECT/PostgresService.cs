using Npgsql;

namespace  OPCUA_PROJECT
{
    /// Implémentation PostgreSQL de IDatabaseService.
    /// Toute la logique SQL est ici — isolée du reste.
    
    public class PostgresService : IDatabaseService
    {
        private readonly string _connectionString;

        public PostgresService(string connectionString)
        {
            _connectionString = connectionString;
        }


        /// Sauvegarde une mesure dans la table measurements.
        public async Task SaveAsync(MeasurementsData data )
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                /*
                 les @ sont la pour evité des injection SQL 
                 */
                await using var cmd = new NpgsqlCommand(@"
                    INSERT INTO measurements
                        (plc_name, variable_name, value, status, source_timestamp)
                    VALUES
                        (@plc, @var, @val, @status, @ts)", conn);

                cmd.Parameters.AddWithValue("@plc", data.PlcName);
                cmd.Parameters.AddWithValue("@var", data.NodeId);
                cmd.Parameters.AddWithValue("@val", data.Value?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@status", data.Status);
                cmd.Parameters.AddWithValue("@ts", data.SourceTimestamp);

                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine($"DB {data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Erreur:{ex.Message}");
            }
        }

        /// Teste la connexion à PostgreSQL au démarrage.
        
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                Console.WriteLine("DB  Connexion PostgreSQL OK");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB  Connexion impossible: {ex.Message}");
                return false;
            }
        }
    }
}