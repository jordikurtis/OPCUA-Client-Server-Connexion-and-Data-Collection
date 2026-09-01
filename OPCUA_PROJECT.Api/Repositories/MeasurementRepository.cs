using Dapper;
using Npgsql;
using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly string _connectionString;

        public MeasurementRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Measurement>>GetLatestAsync()
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    id,
                    variable_id    AS VariableId,
                    value,
                    status,
                    source_timestamp AS SourceTimestamp,
                    inserted_at      AS InsertedAt
                FROM measurements
                ORDER BY inserted_at DESC
                LIMIT 100";

            return await connection.QueryAsync<Measurement>(sql);
        }

        public async Task<IEnumerable<Measurement>> GetHistoryAsync(int variableId, DateTime from, DateTime to)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            const string sql = @" SELECT
                        id,
                        variable_id AS VariableId,
                        value,
                        status,
                        source_timestamp AS SourceTimestamp,
                        inserted_at AS InsertedAt
                        FROM measurements
                        WHERE variable_id = @variableId
                        AND source_timestamp BETWEEN @from AND @to
                        ORDER BY source_timestamp";

            return await connection.QueryAsync<Measurement>( sql, new { variableId,from,to });
        }
    }
}
