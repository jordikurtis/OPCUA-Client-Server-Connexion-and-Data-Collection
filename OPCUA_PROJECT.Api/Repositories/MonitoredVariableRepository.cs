using Dapper;
using Npgsql;
using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories
{
    public class MonitoredVariableRepository: IMonitoredVariableRepository
    {
        private readonly string _connectionString;

        public MonitoredVariableRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<MonitoredVariable>> GetAllAsync()
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    id,
                    plc_name      AS PlcName,
                    group_id      AS GroupId,
                    node_name     AS NodeName,
                    variable_name AS VariableName,
                    node_id       AS NodeId,
                    data_type     AS DataType,
                    enabled
                FROM monitored_variables
                ORDER BY id";

            return await connection.QueryAsync<MonitoredVariable>(sql);
        }

        public async Task<IEnumerable<MonitoredVariable>>GetByPlcNameAsync(string plcName)
        {
            await using var connection =
                new NpgsqlConnection(_connectionString);

            const string sql = @"
        SELECT
            id,
            plc_name      AS PlcName,
            group_id      AS GroupId,
            node_name     AS NodeName,
            variable_name AS VariableName,
            node_id       AS NodeId,
            data_type     AS DataType,
            enabled
        FROM monitored_variables
        WHERE plc_name = @plcName
        ORDER BY id";

            return await connection.QueryAsync<MonitoredVariable>(
                sql,new { plcName });
        }
    }
}
