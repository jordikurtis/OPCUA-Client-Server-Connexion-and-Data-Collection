using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OPCUA_PROJECT;

namespace OPCUA_PROJECT
{
    class Program
    {
        private const string ConnectionString =
            "Host=localhost;Port=5432;Database=OPC_UA Data;" +
            "Username=postgres;Password=Julie alisson";

        static async Task Main()
        {
            Console.WriteLine("=== OPC UA Client — Tenneco Kolben Ring ===\n");

            var logger = new LogService("logs");
            await logger.InfoAsync("OPCUA Client démarré !!! ");

            // 1. Charger la config une première fois — validation "fail-fast"
            //    avant de démarrer quoi que ce soit. PlcConnectionManager
            //    rechargera lui-même en continu une fois lancé.
            var configService = new PostgresConfigService(ConnectionString);
            var initialPlcs = await configService.LoadPlcsAsync();

            if (!initialPlcs.Any())
            {
                await logger.ErrorAsync("Aucun PLC actif trouvé dans plc_configs.");
                Console.WriteLine(" Aucun PLC actif trouvé dans la DB. Vérifie la table plc_configs.");
                return;
            }

            // 2. Cache NodeId -> variable_id/plc_id/IsFaultIndicator
            var variablesService = new MonitoredVariablesService(ConnectionString);
            await variablesService.BuildCacheAsync(initialPlcs, logger);

            // 3. Fehlermeldung — détection des transitions de défaut (Phase 2)
            var faultEventService = new FaultEventService(ConnectionString, logger);

            // 4. Tester la connexion DB (fail-fast)
            var db = new PostgresService(ConnectionString, variablesService, faultEventService, logger);
            if (!await db.TestConnectionAsync())
            {
                await logger.ErrorAsync("Impossible de démarrer la DB - Inaccessible ");
                Console.WriteLine("Impossible de démarrer sans base de données.");
                return;
            }

            Console.WriteLine($"\n{initialPlcs.Count} PLC(s) actif(s) — démarrage du monitoring...\n");

            // 5. Démarrer le gestionnaire de connexions — rechargement config
            //    léger : recharge la config toutes les 30s et démarre/arrête
            //    les connexions individuellement en conséquence.
            var manager = new PlcConnectionManager(
                configService,
                variablesService,
                data => db.SaveAsync(data),
                logger,
                reloadInterval: TimeSpan.FromSeconds(30));

            using var shutdownCts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\nArrêt demandé (Ctrl+C) — fermeture propre des connexions...");
                shutdownCts.Cancel();
            };

            Console.WriteLine("En cours d'exécution — Ctrl+C pour arrêter proprement.\n");
            await manager.RunAsync(shutdownCts.Token);

            Console.WriteLine("Arrêt terminé.");
        }
    }
}
