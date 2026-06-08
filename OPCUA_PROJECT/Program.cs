
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace OPCUA_PROJECT
{
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
        }; 

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



}
    












