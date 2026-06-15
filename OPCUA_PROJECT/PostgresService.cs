using Npgsql;

namespace OPCUA_PROJECT
{
    public class PostgresService : IDatabaseService
    {
        private readonly string _connectionString;

        public PostgresService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SaveAsync(MeasurementsData data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.NodeName) ||
                    string.IsNullOrWhiteSpace(data.VariableName))
                {
                    Console.WriteLine("DB: Table ou colonne non définie, écriture ignorée.");
                    return;
                }

                var sql = $@"
                    INSERT INTO {data.NodeName}
                        (ts, {data.VariableName})
                    VALUES
                        (@ts, @val);";

                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ts", data.SourceTimestamp);
                cmd.Parameters.AddWithValue("@val", data.Value ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine($"DB OK → {data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Erreur: {ex.Message}");
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













/*using Npgsql;

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
/*
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
*/