
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace TennecoOPCUA
{
   static  class Program
    {
        static async Task Main()
        {
            //"opc.tcp://192.168.0.2:4840";
            // "opc.tcp://BRSC-FGFXSQ3.pt.int.tenneco.com:53530/OPCUA/SimulationServer"; // 10.14.67.11 
            // var endpointUrl = "opc.tcp://192.168.0.2:4840";
            // Remplacer avec la vraie IP quand disponible 
            /* cas de figure de un seul PLC a gerer 
             
            const string serverUrl = "opc.tcp://BRSC-FGFXSQ3.pt.int.tenneco.com:53530/OPCUA/SimulationServer";
             */

            try
            {
                // STEP 1 — Configuration
                const string certStore =
                    @"%CommonApplicationData%\OPC Foundation\CertificateStores";

                /*
                 * La création d'une nouvelle configuration d'application consiste 
                 * à définir tous les paramètres nécessaires qui définissent 
                 * - le comportement, 
                 - la communication et 
                 - la sécurisation des connexions de l'application cliente avec le serveur OPC UA. 
                 Ceci est une étape fondamentale lors de l'utilisation de la bibliothèque standard .NET de l'OPC Foundation
                 -creation d'une instance de la Application . 
                (ici est un peu different car la variable config prend tous en parametre qui ensuite ira dans  ^^ApplicationInstance ...   )
                - ces deux methode  " ApplicationInstance " ET  " ApplicationConfiguration " s'occupe du App setup  coté client.
                */

                var config = new ApplicationConfiguration
                {
                    ApplicationName = "OPC UA- TENNECO ",

                    /*
                     * c'est un identifiant universel (URI) unique au monde pour votre instance d'application OPC UA.
                     * -> Son rôle principal est de distinguer cette instance d'application de toute autre application ou instance du réseau.
                     */
                    //Format alternative :-  ApplicationUri = $"urn:{Utils.GetHostName()}:OpcUaClientExample",

                    ApplicationUri = Utils.Format(
                                        @"urn:{0}:OPC UA - JORDI-TENNECO",
                                        System.Net.Dns.GetHostName()),
                    ApplicationType = ApplicationType.Client,

                    ClientConfiguration = new ClientConfiguration
                    {
                        DefaultSessionTimeout = 60000
                    },
                    TransportQuotas = new TransportQuotas
                    {
                        OperationTimeout = 60000
                        /*
                         le temps maximum que le client attendra pour une réponse du serveur pour une seule opération. 
                        Si le serveur ne répond pas dans ce délai, le client déclenche généralement une erreur de dépassement de délai, 
                        ce qui permet à l'application de gérer les problèmes de communication de manière gracieuse
                         */

                        /*
                         IL Y'A d'autres varibale comme 
                        - MaxStringLength (aussi pour serveur)-1048576
                        - MaxByteStringLength 1048576
                        -MaxArrayLength (aussi pour serveur) 65535
                        -MaxMessageSize (aussi pour serveur) 4194304
                        -MaxBufferSize (aussi pour serveur) 65535
                        -channelLifetime (aussi pour serveur) 300000
                        SecurityTokenLifetime (aussi pour serveur) 3600000
                         */
                    },
                    /* alternative mais avoir un fichier PKI dans le dossier du fichier 
                   SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine("pki", "own"),
                    SubjectName = "CN=OpcUaClientExample"
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine("pki", "trusted")
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine("pki", "rejected")
                },
                AutoAcceptUntrustedCertificates = true,
                RejectSHA1SignedCertificates = false
            },

            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 60000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        };
                     */

                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier
                        {// OÚ le certicate de L'instance de l'application est stocké !
                            StoreType = "Directory",
                            StorePath = $"{certStore}\\MachineDefault",
                            SubjectName = "OPC UA - JORDI-TENNECO"
                        },
                        TrustedPeerCertificates = new CertificateTrustList
                        { // oú les certificats de confiance sont stocké !
                            StoreType = "Directory",
                            StorePath = $"{certStore}\\UA Applications"
                        }, 
                        TrustedIssuerCertificates = new CertificateTrustList
                        { // oú la personnee  qui donne le certificat stocke le certificat !(Authorities Certificate )
                            StoreType = "Directory",
                            StorePath = $"{certStore}\\UA Certificate Authorities"
                        },
                        RejectedCertificateStore = new CertificateTrustList
                        { // oú les certificats rejetés sont stocké !  ( Pour les revue ulterieur par l'administrateur ) 
                            StoreType = "Directory",
                            StorePath = $"{certStore}\\RejectedCertificates"
                        },
                        AutoAcceptUntrustedCertificates = true,
                        AddAppCertToTrustedStore = true
                    }
                };

                // STEP 2 — Validation + Certificat

                // API MODERNE — CheckApplicationInstanceCertificate á étésupprimée mais peut etre utilisé ,V 1.4.. .. .. du Paket OPC-UA 
                await config.ValidateAsync(ApplicationType.Client); // meilleur que -->   await config.Validate (.... ) ; 
                // verifie si le certificat est OK ! 

                /*
                 ceci est une inline event handler ecrite en fonction Lamdba
                s ->  refers to the "sender" — the object raising the event , dans ce cas << CertificateValidator.>>
                e -> is an instance of event arguments, usually derived from EventArgs. 
                In certificate validation, it might include information like the certificate being 
                validated and whether validation succeeded.
                 */
                config.CertificateValidator.CertificateValidation += (s, e) =>
                {
                    e.Accept = true;
                };

                
                // Approach pour pour gerer plusieur PLC ..  --------> 
                 
                var plcs = new List<PlcDefinition>
{ 
    new PlcDefinition
    {
        Name = "PLC 1 - Prosys Simulator  ",
        EndpointUrl = "opc.tcp://BRSC-FGFXSQ3.pt.int.tenneco.com:53530/OPCUA/SimulationServer",
        NodesToMonitor = new List<string>
        {
        //"ns=3;i=1007",
        //"ns=3;i=1001",
        //"ns=3;i=1002",
        "ns=3;i=1003",
        //"ns=3;i=1004",
       // "ns=3;i=1005", 
        //"ns=3;i=1006"
        // ...... ajoute ici autant de NodeId que tu veux
        }
        
    },
    

    // plc 2
  /* new PlcDefinition
    {
        Name = "PLC_TEST -- VON Alex ",
        EndpointUrl = "opc.tcp://192.168.0.2:4840",
        NodesToMonitor = new List<string>
        {
           "ns=4;i=56",
            //"ns=4;i=65"
        }
    } */ 
    
};
                /* 
                 
               // Appraoch pour gerer un seul PLC 

                 

                // STEP 3.1 — Endpoint
                Console.WriteLine($"Essaie de ce connecter au Module : {serverUrl}");

                //  Signature corrigée
                var endpointDesc = CoreClientUtils.SelectEndpoint(config,serverUrl, useSecurity: false);
                var endpointConfig = EndpointConfiguration.Create(config);
                var endpoint = new ConfiguredEndpoint(null, endpointDesc, endpointConfig);

                // STEP 4.1 — Creation de la Session
                using var session = await Session.Create(
                     config,
                    endpoint,
                    false, // la variable est - updateBeforeConnect :
                    "OpcUaPoCSession", // la variabale est sessionName : 
                    60000, // La variable est sessionTimeout : 
                    new UserIdentity(new AnonymousIdentityToken()),
                    null); // la variabale est preferredLocales : 

                Console.WriteLine(" Connected!");
                Console.WriteLine($"   Session ID : {session.SessionId}");
                Console.WriteLine($"   Server URI : {session.Endpoint.Server.ApplicationUri}");



                // STEP 5.1 — Subscription + Live Monitoring
                var subscription = CreateSubscription(session);
                AddMonitoredItems(subscription);

                Console.WriteLine("\nAppui sur n'importe Quelle touche sur le Clavier Pour arréter ... !!!");
                Console.ReadKey();


                session.CloseAsync(); // meilleur que --> session.Close() ; 
                Console.WriteLine(" Déconnecter .");

                */


                foreach (var plc in plcs)
                {
                    Console.WriteLine("\n=======================================");
                    Console.WriteLine($" Connexion au Divers PLC : {plc.Name}");
                    Console.WriteLine($" Endpoint         : {plc.EndpointUrl}");
                    Console.WriteLine("===================\n");

                    // STEP 3.n — Endpoint (par PLC) --- Connexion 
                    var endpointDesc = CoreClientUtils.SelectEndpoint(
                        config,
                        plc.EndpointUrl,
                        useSecurity: false);

                    var endpointConfig = EndpointConfiguration.Create(config);
                    var endpoint = new ConfiguredEndpoint(null, endpointDesc, endpointConfig);

                    // STEP 4.n — Session (UNE PAR PLC)
                    var session = await Session.Create(
                        config,
                        endpoint,
                        true, // ----------- true pour une souscription stable 
                        $"Session_{plc.Name}",
                        60000,
                        new UserIdentity(new AnonymousIdentityToken()),
                        null);

                    Console.WriteLine($" [{plc.Name}] Session connectée");

                    // STEP 5.n — Subscription + Live Monitoring
                    var subscription = CreateSubscription(session);
                    AddMonitoredItems(subscription, plc);
                }
                Console.WriteLine("\n Toutes les connexions PLC sont actives.");
                Console.WriteLine(" Appuie sur une touche pour arrêter le programme...");
                Console.ReadKey();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Une Erreur est intervenu ; l'Erreur est    : {ex.Message}");
                Console.WriteLine($" voici les details ;  Details : {ex.StackTrace}");
            }
        }

        
        // ==== SUBSCRIPTION + LIVE MONITORING OPC UA ====
        

        private static Subscription CreateSubscription(Session session)
        {
            var subscription = new Subscription(session.DefaultSubscription)
            {
                PublishingInterval = 3000,   // Apres combien de temps le serveur envoie les données (changeable )
                                             // ici toutes les 3s
                PublishingEnabled = true,
                Priority = 0
            };

            session.AddSubscription(subscription);
            subscription.Create();

            Console.WriteLine("Subscription créée pour une récupération Live des données ");

            return subscription;
        }

        private static void AddMonitoredItems(Subscription subscription , PlcDefinition plc )
        {
            /*
            var nodesToMonitor = new List<string>
    {
       // "ns=3;i=1007",
        "ns=3;i=1001",
        "ns=3;i=1002",
        //"ns=3;i=1003",
        "ns=3;i=1004",
       // "ns=3;i=1005", 
        "ns=3;i=1006" 
        // ...... ajoute ici autant de NodeId que tu veux

    };

            foreach (var node in nodesToMonitor)
            {
                var monitoredItem = new MonitoredItem(subscription.DefaultItem)
                {
                   // StartNodeId = new NodeId(node),
                   StartNodeId = NodeId.Parse(node), // cette approach est plus robust , l'autre marche mais pourrais crash !! 
                    AttributeId = Attributes.Value,
                    SamplingInterval = 0,    // Quand tu met 0 ca veux dire -> envoie les données aussi rapide que possible
                    QueueSize = 10,
                    DiscardOldest = true
                };

                monitoredItem.Notification += OnDataChanged;
                subscription.AddItem(monitoredItem);

                Console.WriteLine($" Monitoring actif des Noeuds  : {node}");
            }

            subscription.ApplyChanges();
            */

            foreach (var node in plc.NodesToMonitor)
            {
                var monitoredItem = new MonitoredItem(subscription.DefaultItem)
                {
                    StartNodeId = NodeId.Parse(node),
                    AttributeId = Attributes.Value,
                    SamplingInterval = 0,//remet a 0
                    QueueSize = 10,
                    DiscardOldest = true,
                   // MonitoringMode = MonitoringMode.Reporting,//enleve cette ligne

                    // ⭐ CONTEXTE PLC + NODE
                    DisplayName = $"{plc.Name}:{node}"
                }; 

                monitoredItem.Notification += OnDataChanged;
                subscription.AddItem(monitoredItem);

                Console.WriteLine($"[{plc.Name}] Monitoring actif : {node}");
            }

            subscription.ApplyChanges();
        }

        private static void OnDataChanged(
            MonitoredItem item,
            MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                Console.WriteLine("--- LIVE UPDATE DES NOEUDS ---");
                Console.WriteLine($"NodeId    : {item.StartNodeId}");
                Console.WriteLine($"Type      :  { value.Value?.GetType().Name}");
                Console.WriteLine($"Valeur    : {value.Value}");
                Console.WriteLine($"Status    : {value.StatusCode}");
                Console.WriteLine($"Timestamp : {value.SourceTimestamp}");
                

            }
        }
    }
}


