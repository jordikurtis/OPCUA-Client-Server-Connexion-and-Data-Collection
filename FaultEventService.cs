using Npgsql;
using System;
using System.Threading.Tasks;
namespace OPCUA_PROJECT
{
    /// <summary>
    /// Détecte les transitions d'état d'une variable marquée comme
    /// indicateur de défaut (IsFaultIndicator) et gère le cycle de vie des
    /// lignes fault_events (ouverture/fermeture).
    ///
    /// Convention actuelle (cas "tags simples", à confirmer avec Witte) :
    /// valeur == "0" -> pas de défaut ; valeur != "0" -> défaut actif, la
    /// valeur elle-même sert de fault_code. Si TIA Portal expose plutôt de
    /// vraies OPC-UA Alarms & Conditions, CETTE CLASSE est le seul endroit
    /// à revoir — le reste (fault_events, API, frontend) reste valable.
    /// </summary>
    public class FaultEventService
    {
        private readonly string _connectionString;
        private readonly LogService? _logger;
        // Convention "pas de défaut" — centralisée ici pour être facile à
        // changer/étendre sans toucher au reste de la logique.
        private const string NoFaultValue = "0";
        // Un verrou par (plcId, variableId) : sans ça, deux mesures de la
        // même variable arrivant à quelques millisecondes d'écart peuvent
        // toutes les deux lire "aucun défaut actif" avant que la première
        // ait fini d'écrire, et ouvrir chacune leur propre ligne —
        // plusieurs défauts "actifs" pour la même variable en même temps.
        // Portée par variable (pas globale) pour ne pas sérialiser des
        // variables indépendantes entre elles.
        private readonly System.Collections.Concurrent.ConcurrentDictionary<(int PlcId, int VariableId), SemaphoreSlim> _locks = new();
        public FaultEventService(string connectionString, LogService? logger = null)
        {
            _connectionString = connectionString;
            _logger = logger;
        }
        public async Task HandleValueAsync(int plcId, int variableId, string? value, DateTime timestamp)
        {
            var gate = _locks.GetOrAdd((plcId, variableId), _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync();
            try
            {
                await HandleValueLockedAsync(plcId, variableId, value, timestamp);
            }
            finally
            {
                gate.Release();
            }
        }
        private async Task HandleValueLockedAsync(int plcId, int variableId, string? value, DateTime timestamp)
        {
            var normalizedValue = (value ?? "").Trim();
            var hasFault = normalizedValue.Length > 0 && normalizedValue != NoFaultValue;
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            var active = await GetActiveEventAsync(conn, plcId, variableId);
            if (!hasFault)
            {
                if (active != null)
                {
                    await CloseEventAsync(conn, active.Value.Id, timestamp);
                    if (_logger != null)
                        await _logger.InfoAsync($"[Fehlermeldung] Défaut résolu (code={active.Value.FaultCode}).");
                }
                return;
            }
            if (active == null)
            {
                await OpenEventAsync(conn, plcId, variableId, normalizedValue, timestamp);
                if (_logger != null)
                    await _logger.WarningAsync($"[Fehlermeldung] Nouveau défaut (code={normalizedValue}).");
                return;
            }
            if (active.Value.FaultCode == normalizedValue)
            {
                // Même défaut toujours en cours — on évite de spammer
                // fault_events à chaque cycle de publication.
                return;
            }
            // Code différent alors qu'un défaut était déjà actif : l'ancien
            // est considéré résolu, un nouveau commence (utile pour les
            // statistiques type "20x en Störung sur une shift").
            await CloseEventAsync(conn, active.Value.Id, timestamp);
            await OpenEventAsync(conn, plcId, variableId, normalizedValue, timestamp);
            if (_logger != null)
                await _logger.WarningAsync(
                    $"[Fehlermeldung] Défaut changé ({active.Value.FaultCode} -> {normalizedValue}).");
        }
        private static async Task<(int Id, string FaultCode)?> GetActiveEventAsync(
            NpgsqlConnection conn, int plcId, int variableId)
        {
            await using var cmd = new NpgsqlCommand(@"
                SELECT id, fault_code
                FROM fault_events
                WHERE plc_id = @plcId AND variable_id = @variableId AND cleared_at IS NULL
                ORDER BY occurred_at DESC
                LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@plcId", plcId);
            cmd.Parameters.AddWithValue("@variableId", variableId);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return (reader.GetInt32(0), reader.GetString(1));
            return null;
        }
        private static async Task OpenEventAsync(
            NpgsqlConnection conn, int plcId, int variableId, string faultCode, DateTime occurredAt)
        {
            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO fault_events (plc_id, variable_id, fault_code, severity, occurred_at)
                VALUES (@plcId, @variableId, @faultCode, 'Error', @occurredAt)", conn);
            cmd.Parameters.AddWithValue("@plcId", plcId);
            cmd.Parameters.AddWithValue("@variableId", variableId);
            cmd.Parameters.AddWithValue("@faultCode", faultCode);
            cmd.Parameters.AddWithValue("@occurredAt", occurredAt);
            await cmd.ExecuteNonQueryAsync();
        }
        private static async Task CloseEventAsync(NpgsqlConnection conn, int id, DateTime clearedAt)
        {
            await using var cmd = new NpgsqlCommand(@"
                UPDATE fault_events
                SET cleared_at = @clearedAt
                WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@clearedAt", clearedAt);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}

/*
 * using Npgsql;
using System;
using System.Threading.Tasks;

namespace OPCUA_PROJECT
{
    /// <summary>
    /// Détecte les transitions d'état d'une variable marquée comme
    /// indicateur de défaut (IsFaultIndicator) et gère le cycle de vie des
    /// lignes fault_events (ouverture/fermeture).
    ///
    /// Convention actuelle (cas "tags simples", à confirmer avec Witte) :
    /// valeur == "0" -> pas de défaut ; valeur != "0" -> défaut actif, la
    /// valeur elle-même sert de fault_code. Si TIA Portal expose plutôt de
    /// vraies OPC-UA Alarms & Conditions, CETTE CLASSE est le seul endroit
    /// à revoir — le reste (fault_events, API, frontend) reste valable.
    /// </summary>
    public class FaultEventService
    {
        private readonly string _connectionString;
        private readonly LogService? _logger;

        // Convention "pas de défaut" — centralisée ici pour être facile à
        // changer/étendre sans toucher au reste de la logique.
        private const string NoFaultValue = "0";

        public FaultEventService(string connectionString, LogService? logger = null)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task HandleValueAsync(int plcId, int variableId, string? value, DateTime timestamp)
        {
            var normalizedValue = (value ?? "").Trim();
            var hasFault = normalizedValue.Length > 0 && normalizedValue != NoFaultValue;

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var active = await GetActiveEventAsync(conn, plcId, variableId);

            if (!hasFault)
            {
                if (active != null)
                {
                    await CloseEventAsync(conn, active.Value.Id, timestamp);
                    if (_logger != null)
                        await _logger.InfoAsync($"[Fehlermeldung] Défaut résolu (code={active.Value.FaultCode}).");
                }
                return;
            }

            if (active == null)
            {
                await OpenEventAsync(conn, plcId, variableId, normalizedValue, timestamp);
                if (_logger != null)
                    await _logger.WarningAsync($"[Fehlermeldung] Nouveau défaut (code={normalizedValue}).");
                return;
            }

            if (active.Value.FaultCode == normalizedValue)
            {
                // Même défaut toujours en cours — on évite de spammer
                // fault_events à chaque cycle de publication.
                return;
            }

            // Code différent alors qu'un défaut était déjà actif : l'ancien
            // est considéré résolu, un nouveau commence (utile pour les
            // statistiques type "20x en Störung sur une shift").
            await CloseEventAsync(conn, active.Value.Id, timestamp);
            await OpenEventAsync(conn, plcId, variableId, normalizedValue, timestamp);

            if (_logger != null)
                await _logger.WarningAsync(
                    $"[Fehlermeldung] Défaut changé ({active.Value.FaultCode} -> {normalizedValue}).");
        }

        private static async Task<(int Id, string FaultCode)?> GetActiveEventAsync(
            NpgsqlConnection conn, int plcId, int variableId)
        {
            await using var cmd = new NpgsqlCommand(@"
                SELECT id, fault_code
                FROM fault_events
                WHERE plc_id = @plcId AND variable_id = @variableId AND cleared_at IS NULL
                ORDER BY occurred_at DESC
                LIMIT 1", conn);

            cmd.Parameters.AddWithValue("@plcId", plcId);
            cmd.Parameters.AddWithValue("@variableId", variableId);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return (reader.GetInt32(0), reader.GetString(1));

            return null;
        }

        private static async Task OpenEventAsync(
            NpgsqlConnection conn, int plcId, int variableId, string faultCode, DateTime occurredAt)
        {
            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO fault_events (plc_id, variable_id, fault_code, severity, occurred_at)
                VALUES (@plcId, @variableId, @faultCode, 'Error', @occurredAt)", conn);

            cmd.Parameters.AddWithValue("@plcId", plcId);
            cmd.Parameters.AddWithValue("@variableId", variableId);
            cmd.Parameters.AddWithValue("@faultCode", faultCode);
            cmd.Parameters.AddWithValue("@occurredAt", occurredAt);

            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task CloseEventAsync(NpgsqlConnection conn, int id, DateTime clearedAt)
        {
            await using var cmd = new NpgsqlCommand(@"
                UPDATE fault_events
                SET cleared_at = @clearedAt
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@clearedAt", clearedAt);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
*/