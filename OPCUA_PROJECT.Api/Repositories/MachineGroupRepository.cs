using Dapper;
using Npgsql;
using OPCUA_PROJECT.Api.Models;

namespace OPCUA_PROJECT.Api.Repositories;

public class MachineGroupRepository : IMachineGroupRepository
{
    private readonly string _connectionString;

    public MachineGroupRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<MachineGroup>> GetAllAsync()
    {
        await using var connection =
            new NpgsqlConnection(_connectionString);

        const string sql = @"
            SELECT
                id,
                name,
                description
            FROM machinegroups
            ORDER BY id";

        return await connection.QueryAsync<MachineGroup>(sql);
    }
}
