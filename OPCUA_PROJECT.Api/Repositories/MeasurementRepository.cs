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

        public async Task<IEnumerable<Measurement>> GetLatestAsync()
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            // DISTINCT ON (variable_id) : garde une seule ligne par variable —
            // la plus récente (grâce à ORDER BY variable_id, source_timestamp DESC).
            // Évite que les variables Echtzeit (haute fréquence) écrasent les
            // variables lentes (event-based) dans le résultat.
            const string sql = @"
                SELECT DISTINCT ON (variable_id)
                    id,
                    variable_id      AS VariableId,
                    value,
                    status,
                    source_timestamp AS SourceTimestamp,
                    inserted_at      AS InsertedAt
                FROM measurements
                ORDER BY variable_id, source_timestamp DESC";

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

            return await connection.QueryAsync<Measurement>(sql, new { variableId, from, to });
        }
    }
}
