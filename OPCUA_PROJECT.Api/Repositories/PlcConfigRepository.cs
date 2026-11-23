using Dapper;
using Npgsql;
using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public class PlcConfigRepository : IPlcConfigRepository
    {
        private readonly string _connectionString;

        public PlcConfigRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<PlcConfig>> GetAllAsync()
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    id,
                    name,
                    endpoint_url AS EndpointUrl,
                    enabled,
                    group_id AS GroupId
                FROM plc_configs
                ORDER BY id";

            return await connection.QueryAsync<PlcConfig>(sql);
        }

        public async Task<PlcConfig?> GetByIdAsync(int id)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    id,
                    name,
                    endpoint_url AS EndpointUrl,
                    enabled,
                    group_id AS GroupId
                FROM plc_configs
                WHERE id = @id";

            return await connection.QueryFirstOrDefaultAsync<PlcConfig>(
                sql,
                new { id });
        }

        public async Task<int> CreateAsync(PlcConfig plc)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO plc_configs
                    (name, endpoint_url, enabled, group_id, config_json)
                VALUES
                    (@Name, @EndpointUrl, @Enabled, @GroupId, '{""nodes"": []}'::jsonb)
                RETURNING id";

            return await connection.ExecuteScalarAsync<int>(sql, plc);
        }

        public async Task<bool> UpdateAsync(int id, PlcConfig plc)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            // NOUVEAU : si EndpointUrl est vide/null, on garde l'ancienne
            // valeur (COALESCE + NULLIF) au lieu de l'écraser. Nécessaire
            // car GET ne renvoie jamais EndpointUrl (masqué côté sécurité) —
            // sans ça, l'Admin serait obligé de le re-saisir à chaque édition,
            // même pour juste activer/désactiver un PLC.
            const string sql = @"
                UPDATE plc_configs
                SET
                    name = @Name,
                    endpoint_url = COALESCE(NULLIF(@EndpointUrl, ''), endpoint_url),
                    enabled = @Enabled,
                    group_id = @GroupId
                WHERE id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                plc.Name,
                plc.EndpointUrl,
                plc.Enabled,
                plc.GroupId
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = "DELETE FROM plc_configs WHERE id = @id";

            var rowsAffected = await connection.ExecuteAsync(sql, new { id });
            return rowsAffected > 0;
        }
    }
}

