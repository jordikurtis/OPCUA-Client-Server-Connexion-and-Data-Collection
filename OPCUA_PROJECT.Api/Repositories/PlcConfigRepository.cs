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

    }
}
