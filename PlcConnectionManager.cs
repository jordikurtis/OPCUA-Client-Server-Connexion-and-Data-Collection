using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUA_PROJECT
{
    public  class PlcConnectionManager
    {
        /// <summary>
        /// Rechargement config "léger" (dernière pièce de la Phase 2) : recharge
        /// la config toutes les X secondes et démarre/arrête les connexions PLC
        /// individuellement en conséquence — sans redémarrer tout le programme.
        ///
        /// Volontairement simple (pas de ConcurrentDictionary/PlcManager complet,
        /// réservé à la Phase 6 si le multi-machine en montre le besoin) :
        /// - PLC nouvellement actif (ou réactivé)         -> démarré
        /// - PLC désactivé/supprimé                        -> arrêté
        /// - PLC dont l'endpoint ou les variables ont changé -> redémarré (pas
        ///   de diff fin, on relance tout le PLC — plus simple, largement
        ///   suffisant à ce stade du projet)
        /// - PLC inchangé                                   -> laissé tel quel
        /// </summary>
       
            private readonly PostgresConfigService _configService;
            private readonly MonitoredVariablesService _variablesService;
            private readonly Func<MeasurementsData, Task> _onDataReceived;
            private readonly LogService _logger;
            private readonly TimeSpan _reloadInterval;
            private class ActiveConnection
            {
                public required PlcDefinition Plc { get; init; }
                public required IOpcUaClient Client { get; init; }
                public required Task MonitoringTask { get; init; }
                public required CancellationTokenSource Cts { get; init; }
                public required string ConfigSignature { get; init; }
            }
            private readonly Dictionary<int, ActiveConnection> _active = new();
            public PlcConnectionManager( PostgresConfigService configService, MonitoredVariablesService variablesService,Func<MeasurementsData, Task> onDataReceived,
                LogService logger,
                TimeSpan? reloadInterval = null)
            {
            _configService = configService;
            _variablesService = variablesService;
            _onDataReceived = onDataReceived;
            _logger = logger;
            _reloadInterval = reloadInterval ?? TimeSpan.FromSeconds(30);
            }
        /// <summary>
        /// Boucle principale — tourne jusqu'à ce que shutdownToken soit
        /// annulé (ex: Ctrl+C, voir Program.cs). Premier chargement immédiat,
        /// puis un cycle toutes les _reloadInterval.
        /// </summary>
        public async Task RunAsync(CancellationToken shutdownToken)
            {
                while (!shutdownToken.IsCancellationRequested)
                {
                    try
                    {
                    await ReloadOnceAsync();
                    }
                catch (Exception ex)
                {
                        // Une erreur pendant un cycle de rechargement ne doit pas
                        // tuer le programme — on logue et on réessaiera au
                        // prochain cycle.
                        await _logger.ErrorAsync($"[PlcConnectionManager] Erreur pendant le rechargement : {ex.Message}");
                    }
                    try
                    {
                    await Task.Delay(_reloadInterval, shutdownToken);
                    }
                catch (OperationCanceledException)
                    {
                        break;
                    }
                }
                await StopAllAsync();
            }
            private async Task ReloadOnceAsync()
            {
                var plcs = await _configService.LoadPlcsAsync();
                // Rafraîchit le cache NodeId -> variable_id pour tous les PLCs
                // actifs, y compris ceux déjà en cours (variables ajoutées via
                // l'Admin depuis le dernier cycle).
                await _variablesService.BuildCacheAsync(plcs, _logger);
                var currentIds = plcs.Select(p => p.Id).ToHashSet();
                // 1. Arrêter les PLCs qui ne sont plus actifs (désactivés/supprimés)
                foreach (var id in _active.Keys.Except(currentIds).ToList())
                {
                    await StopConnectionAsync(id);
                }
                // 2. Démarrer les nouveaux, redémarrer ceux dont la config a changé
                foreach (var plc in plcs)
                {
            var signature = BuildSignature(plc);
                if (_active.TryGetValue(plc.Id, out var existing))
                {
                    if (existing.ConfigSignature == signature)
                        continue; // inchangé — on ne touche pas à une connexion stable
                    await _logger.InfoAsync("Config modifiée détectée, redémarrage.", plc.Name);
            await StopConnectionAsync(plc.Id);
        }
        await StartConnectionAsync(plc, signature);
                }
            }
        private async Task StartConnectionAsync(PlcDefinition plc, string signature)
        {
            var cts = new CancellationTokenSource();
            var client = new OPCUAService(_logger);
            var task = client.ConnectAndMonitorAsync(plc, _onDataReceived, cts.Token);
            _active[plc.Id] = new ActiveConnection
            {
                Plc = plc,
                Client = client,
                MonitoringTask = task,
                Cts = cts,
                ConfigSignature = signature
            };
            await _logger.InfoAsync("Connexion démarrée.", plc.Name);
        }
        private async Task StopConnectionAsync(int plcId)
        {
            if (!_active.TryGetValue(plcId, out var connection))
                return;
            connection.Cts.Cancel();
            try
            {
                await connection.Client.DisconnectAsync();
            await connection.MonitoringTask;
            }
            catch (OperationCanceledException)
            {
                // Attendu — la tâche s'arrête via le CancellationToken
            }
            catch (Exception ex)
            {
                await _logger.WarningAsync($"Erreur pendant l'arrêt : {ex.Message}", connection.Plc.Name);
            }
            connection.Cts.Dispose();
            _active.Remove(plcId);
            await _logger.InfoAsync("Connexion arrêtée.", connection.Plc.Name);
        }
        private async Task StopAllAsync()
        {
            foreach (var id in _active.Keys.ToList())
            {
                await StopConnectionAsync(id);
            }
        }
        // Signature simple pour détecter un changement de config (endpoint,
        // ou liste de nodes/variables) — recalculée à chaque cycle et
        // comparée à la précédente pour décider s'il faut redémarrer.
        private static string BuildSignature(PlcDefinition plc)
        {
            var parts = new List<string> { plc.EndpointUrl };
            foreach (var node in plc.Nodes.OrderBy(n => n.Name))
            {
                foreach (var v in node.Variables.OrderBy(v => v.NodeId))
                {
                    parts.Add($"{node.Name}:{v.NodeId}:{v.DataType}:{v.Enabled}");
                }
            }
            return string.Join("|", parts);
        }
    }

}

