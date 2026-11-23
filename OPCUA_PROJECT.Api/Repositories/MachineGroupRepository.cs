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
            FROM machine_groups
            ORDER BY id";

        return await connection.QueryAsync<MachineGroup>(sql);
    }

    public async Task<MachineGroup?> GetByIdAsync(int id)
    {
        await using var connection =
            new NpgsqlConnection(_connectionString);

        const string sql = @"
            SELECT
                id,
                name,
                description
            FROM machine_groups
            WHERE id = @id";

        return await connection.QueryFirstOrDefaultAsync<MachineGroup>(sql, new { id });
    }

    public async Task<int> CreateAsync(MachineGroup group)
    {
        await using var connection =
            new NpgsqlConnection(_connectionString);

        const string sql = @"
            INSERT INTO machine_groups (name, description)
            VALUES (@Name, @Description)
            RETURNING id";

        return await connection.ExecuteScalarAsync<int>(sql, group);
    }

    public async Task<bool> UpdateAsync(int id, MachineGroup group)
    {
        await using var connection =
            new NpgsqlConnection(_connectionString);

        const string sql = @"
            UPDATE machine_groups
            SET name = @Name, description = @Description
            WHERE id = @Id";

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            group.Name,
            group.Description
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection =
            new NpgsqlConnection(_connectionString);

        // Les PLCs rattachés à ce groupe ne sont PAS supprimés — leur
        // group_id passe à NULL (contrainte FK sans CASCADE). Comportement
        // volontaire : supprimer un groupe ne doit jamais supprimer des
        // machines par effet de bord.
        const string sql = "DELETE FROM machine_groups WHERE id = @id";

        var rowsAffected = await connection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }
}
