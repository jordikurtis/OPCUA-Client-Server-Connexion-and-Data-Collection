using System;
using System.Linq;
using System.Threading.Tasks;
using OPCUA_PROJECT;

namespace OPCUA_PROJECT
{
    class Program
    {
        // Connection String PostgreSQL 
        private const string ConnectionString =
            "Host=localhost;Port=5432;Database=OPC_UA Data;" +
            "Username=postgres;Password=Julie alisson";

        static async Task Main()
        {
            Console.WriteLine("=== OPC UA Client — Tenneco Kolben Ring ===\n");

            //logger partagé entre tous  les PLC's 
            var logger = new LogService("logs"); // pour changer le chamin en production ; modifie ceci et le chemin recommander voire donner 
            await logger.InfoAsync("OPCUA Client démarré !!! ");


            //  1. Tester la connexion PostgreSQL 
            var db = new PostgresService(ConnectionString);
            if (!await db.TestConnectionAsync())
            {
                //ligne ajouter , class LogService
                await logger.ErrorAsync("Impossible de Démarrer la DB - Inaccessible ");

                Console.WriteLine(" Impossible de démarrer sans base de données.");
                return;
            }

            // 2. Charger la config depuis PostgreSQL
            //    Plus de fichier JSON — tout vient de la DB
            var configService = new PostgresConfigService(ConnectionString);
            var plcs = await configService.LoadPlcsAsync();

            if (!plcs.Any())
            {
                //ligne ajouter , class LogService
                await logger.ErrorAsync("Aucun PLC actif trouvé dans  Plc_configs.");

                Console.WriteLine(" Aucun PLC actif trouvé dans la DB. Vérifie la table plc_configs.");
                return;
            }

            Console.WriteLine($"\n{plcs.Count} PLC(s) actif(s) — démarrage du monitoring...\n");

            // 3. Connecter tous les PLCs en parallèle
            var tasks = plcs.Select(plc =>
            {
                var client = new OPCUAService(logger); // //ligne ajouter , class LogService , logger passer en parametre 
                return client.ConnectAndMonitorAsync(
                    plc,
                    data => db.SaveAsync(data));
            });

            await Task.WhenAll(tasks);

            Console.WriteLine("\nAppuie sur une touche pour arrêter...");
            Console.ReadKey();
        }
    }
}

///////////////////////// Celui avec ConfigService.cs 
/*
using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace OPCUA_PROJECT
{
      class Program
        {
            static async Task Main()
            {
                Console.WriteLine("=== OPC UA Client — Tenneco Kolben Ring ===\n");

                // 1️---- Charger la configuration JSON
                var configService = new ConfigServiceJSON("PROSY_JSON.json"); // PROSY_JSON.json OPCUA_NodeId_Config.json

            var config = configService.LoadConfig();

                if (!config.Plcs.Any())
                {
                    Console.WriteLine("Aucun PLC actif trouvé. Arrêt.");
                    return;
                }

                Console.WriteLine($"{config.Plcs.Count} PLC(s) actif(s) chargé(s).\n");

                // 2️ -->  Initialiser la base de données depuis la config
                IDatabaseService db = config.Database.Provider switch
                {
                    "PostgreSQL" => new PostgresService(config.Database.ConnectionString),
                    _ => throw new Exception($"Provider DB non supporté : {config.Database.Provider}")
                };

                if (!await db.TestConnectionAsync())
                {
                    Console.WriteLine("Impossible de démarrer sans base de données.");
                    return;
                }

                // 3️--->  Démarrer les clients OPC UA (un par PLC, en parallèle)
                var tasks = config.Plcs.Select(plc =>
                {
                    var client = new OPCUAService();
                    return client.ConnectAndMonitorAsync(
                        plc,
                        data => db.SaveAsync(data)
                    );
                });

                await Task.WhenAll(tasks);

                Console.WriteLine("\nApplication lancée. CTRL+C pour arrêter.");
                await Task.Delay(-1);
            }
        }
    }
*/

////////////////////
/*
    class Program
    {
        // ── Connection String PostgreSQL ──────────────────────
        private const string ConnectionString =
            "Host=localhost;Port=5432;Database=OPC_UA Data;" +
            "Username=postgres;Password=Julie alisson";

        // ── Liste des PLCs à surveiller ───────────────────────
        private static readonly List<PlcDefinition> Plcs = new()
        {
          /*  new PlcDefinition
            {
                Name = "PLC1_Prosys_Simulator",
                 EndpointUrl  = "opc.tcp://BRSC-FGFXSQ3.pt.int.tenneco.com:53530/OPCUA/SimulationServer",

 
                NodesToMonitor = new List<string>
                {
                    "ns=3;i=1001",
               "ns=3;i=1003"
                   //ns=3;i=1007",
                }
            },
            */
// ── Décommenter quand le vrai module sera accessible

/* new PlcDefinition
 {
     Name        = "Stossschleifen Machine ",
     EndpointUrl = "opc.tcp://10.14.67.11:4840",
     NodesToMonitor = new List<string>
     {
         "ns=3;s=ns=3;s=\"Stoßschleifmaschine\".\"Antrieb_Eindrückstempel\".\"Drehmomente\"",
         "ns=2;s=VotreNodeId2",
     }
 },
*/
//}; 
/*
        static async Task Main()
        {
            Console.WriteLine("=== OPC UA Client — Tenneco Kolben Ring ===");
            Console.WriteLine($"    {Plcs.Count} PLC(s) configuré(s)\n");


            // 1.1 Ligne ajouter ----  Charge la config depuis le fichier JSON //

            var configService = new ConfigServiceJSON("OPCUA_NodeId_Config.json");
            var plcs = configService.LoadPlcs();

            if (plcs.Count == 0) {
                Console.WriteLine("keine Active PLCS gefunden !! ");
                return;
            
            }
            Console.WriteLine($"\n{plcs.Count} PLC(s) à connecter.\n");






            // ── 1. Tester la connexion PostgreSQL ─────────────
            var db = new PostgresService(ConnectionString);
            if (!await db.TestConnectionAsync())
            {
                Console.WriteLine(" Impossible de démarrer sans base de données.");
                return;
            }

            // ── 2. Connecter tous les PLCs en parallèle ───────
            var tasks = Plcs.Select(plc =>
            {
                var client = new OPCUAService();
                return client.ConnectAndMonitorAsync(
                    plc,
                    // Callback : appelé à chaque nouvelle valeur
                    data => db.SaveAsync(data));
            });

            await Task.WhenAll(tasks);

            Console.WriteLine("\nAppuie sur une touche pour arrêter...");
            Console.ReadKey();
        }
    }

*/















