using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace OPCUA_PROJECT
{
    public class OPCUAService : IOpcUaClient, IDisposable //  IDiposable est tout en bas !! 
    {

        //logger 
        private readonly LogService _logger;

        public OPCUAService(LogService logger)
        {
            _logger = logger;
        }



        /// Implémentation du client OPC UA.
        /// Gère connexion, session, subscription et monitoring pour UN PLC.
        /// Appelée en parallèle pour chaque PLC.


        private Session? _session;
        private Subscription? _subscription;
        private bool _disposed;

        private ConfiguredEndpoint? _endpoint;
        private ApplicationConfiguration? _appConfig;

        //  Watchdog 
        // Stockés pour pouvoir recréer la session + subscription
        // lors d'une reconnexion automatique
        private PlcDefinition? _plc;
        private Func<MeasurementsData, Task>? _onDataReceived;
        private PlcStatus _currentStatus = PlcStatus.Offline;

        // SemaphoreSlim : évite plusieurs reconnexions simultanées
        // 1 = une seule tentative à la fois autorisée
        private readonly SemaphoreSlim _reconnectLock = new SemaphoreSlim(1, 1);

        // Délai initial entre tentatives (doublé à chaque échec)
        private const int InitialRetryDelaySeconds = 5;
        private const int MaxRetryDelaySeconds = 60;

        // Interface 
        public bool IsConnected
        {
            get
            {
                if (_session != null)
                    return _session.Connected;
                else
                    return false;
            }
        }

        // public bool IsConnected => _session?.Connected ?? false;

        /*
         ***  l'expression  avec  => _session ... au-dessus est l'equivalent de ceci ou 

     public bool IsConnected
    {

    get { return _session?.Connected ?? false; }

    }
      ***  _session?.Connected – Safely checks if _session is not null; if it exists, it reads the Connected property.??
      ***  ?. safely accesses the Connected property only if _session is not null 
      ***  if _session is null, _session?.connected evaluates to null instead of throwing a NullReferenceException
      ***  ?? false :- This operator provides a fallback value if the left-hand side is null.
      ***  In the expression _session?.Connected ?? false, if _session is null, _session?.Connected returns null, 
      ***  so the ?? false ensures IsConnected will return false
      ***  If _session is not null, the value of _session.Connected is returned 
      ***   ?? false – If _session is null (so .Connected cannot be read), it defaults to false
      ***  This line defines a read-only boolean property that safely checks whether a session is connected
      ***   It guarantees false if _session is null, avoiding runtime errors


      }*/



        // ── Statuts possibles du PLC ─────────────────────────
        public enum PlcStatus
        {
            Online,        //  connecté, données reçues
            Offline,       //  connexion perdue
            Reconnecting   //  tentative de reconnexion en cours
        }

        
        // CONNEXION PRINCIPALE
       
        public async Task ConnectAndMonitorAsync(
            PlcDefinition plc,
            Func<MeasurementsData, Task> onDataReceived)
        {
            // Stockage pour la reconnexion automatique
            _plc = plc;
            _onDataReceived = onDataReceived;

            Console.WriteLine($"\n[{plc.Name}] Connexion à {plc.EndpointUrl}...");

            // Config OPC-UA (une seule fois au démarrage)
            _appConfig = await BuildConfigAsync();

            // Endpoint (stocké pour réutilisation lors du reconnect)
            var endpointDesc = CoreClientUtils.SelectEndpoint( _appConfig, plc.EndpointUrl, useSecurity: false);
            var endpointConfig = EndpointConfiguration.Create(_appConfig);
            _endpoint = new ConfiguredEndpoint(null, endpointDesc, endpointConfig);

            // Création session + subscription
            await CreateSessionAsync();
            await CreateSubscriptionAsync();

            // Statut initial
            SetStatus(PlcStatus.Online);

            // Garder la tâche active
            await Task.Delay(Timeout.Infinite);
        }

         
        // CRÉATION DE LA SESSION
        // Appelée au démarrage ET à chaque reconnexion

        private async Task CreateSessionAsync()
        {
            _session = await Session.Create(
                _appConfig!,
                _endpoint!,
                true,
                $"Session_{_plc!.Name}",
                60000,
                new UserIdentity(new AnonymousIdentityToken()),
                null);

            //  Watchdog : brancher KeepAlive sur la nouvelle session ──
            _session.KeepAlive += OnKeepAlive;

            Log($"[SESSION]  Connecté — ID: {_session.SessionId}");
        }

        // CRÉATION DE LA SUBSCRIPTION + MONITORED ITEMS
        // Appelée au démarrage ET à chaque reconnexion

        private async Task CreateSubscriptionAsync()
        {
            _subscription = new Subscription(_session!.DefaultSubscription)
            {
                PublishingInterval = 3000,
                PublishingEnabled = true,
                Priority = 0
            };

            _session!.AddSubscription(_subscription);
            _subscription.Create();

            Log($"[SUBSCRIPTION] Créée pour {_plc!.Name}");

            foreach (var node in _plc!.Nodes)
            {
                foreach (var variable in node.Variables.Where(v => v.Enabled))
                {
                    var item = new MonitoredItem(_subscription.DefaultItem)
                    {
                        StartNodeId = NodeId.Parse(variable.NodeId),
                        AttributeId = Attributes.Value,
                        SamplingInterval = 0,
                        QueueSize = 10,
                        DiscardOldest = true,
                        DisplayName = $"{_plc.Name}.{node.Name}.{variable.Name}"
                    };

                    // Capture du contexte pour le handler
                    var plcName = _plc.Name;
                    var nodeName = node.Name;
                    var variableName = variable.Name;
                    var nodeId = variable.NodeId;

                    item.Notification += (mi, e) =>
                    {
                        foreach (var value in mi.DequeueValues())
                        {
                            var data = new MeasurementsData
                            {
                                PlcName = plcName,
                                NodeName = nodeName,
                                VariableName = variableName,
                                NodeId = nodeId,
                                Value = value.Value,
                                Status = value.StatusCode.ToString(),
                                SourceTimestamp = value.SourceTimestamp
                            };

                            Console.WriteLine(data);
                            _ = _onDataReceived!(data);
                        }
                    };

                    _subscription.AddItem(item);
                    Log($"[MONITORING] Actif : {node.Name}.{variable.Name}");
                }
            }

            _subscription.ApplyChanges();
        }


        // WATCHDOG — KeepAlive Handler
        // Déclenché automatiquement par la librairie OPC-UA
        // à intervalle régulier pour vérifier la connexion
        // code ajouter aussi qui differencie INFO et ERRORS logServcie INFO , ERRORS 
        private void OnKeepAlive(ISession session, KeepAliveEventArgs e)
        {
            if (ServiceResult.IsBad(e.Status))
            {
                // Connexion perdue - déclencher reconnexion
                Log($"[WATCHDOG] KeepAlive échoué — Status: {e.Status}", LogService.LogLevel.ERROR);
                SetStatus(PlcStatus.Offline);

                // Fire-and-forget : ne bloque pas le thread KeepAlive
                _ = TriggerReconnectAsync();
            }
            // Si statut OK - rien à faire, connexion active
        }


        // RECONNEXION AUTOMATIQUE
        // Tentatives avec délai croissant (5s → 10s → 20s → max 60s)
        // code aussi qui differencie INFO et ERRORS logServcie INFO , ERRORS 
        private async Task TriggerReconnectAsync()
        {
            // Si une reconnexion est déjà en cours → on sort
            // WaitAsync(0) = essai non bloquant
            if (!await _reconnectLock.WaitAsync(0))
            {
                Log("[WATCHDOG] Reconnexion déjà en cours — ignoré.");
                return;
            }

            try
            {
                SetStatus(PlcStatus.Reconnecting);
                int delaySeconds = InitialRetryDelaySeconds;
                int attempt = 1;

                while (!IsConnected)
                {
                    Log($"[WATCHDOG] Tentative {attempt} dans {delaySeconds}s... ", LogService.LogLevel.INFO);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

                    try
                    {
                        // Nettoyer l'ancienne session avant de recréer
                        await CleanupSessionAsync();

                        // Recréer session + subscription
                        await CreateSessionAsync();
                        await CreateSubscriptionAsync();

                        SetStatus(PlcStatus.Online);
                        Log($"[WATCHDOG]  Reconnexion réussie après {attempt} tentative(s)." , LogService.LogLevel.INFO);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log($"[WATCHDOG]  Tentative {attempt} échouée : {ex.Message}", LogService.LogLevel.ERROR);

                        // Délai croissant — plafonné à MaxRetryDelaySeconds
                        delaySeconds = Math.Min(delaySeconds * 2, MaxRetryDelaySeconds);
                        attempt++;
                    }
                }
            }
            finally
            {
                // Toujours libérer le verrou — même en cas d'exception
                _reconnectLock.Release();
            }
        }


        // NETTOYAGE — Ferme proprement session + subscription
        // Appelé avant chaque tentative de reconnexion
        private async Task CleanupSessionAsync()
        {
            try
            {
                if (_subscription != null)
                {
                    await _subscription.DeleteAsync(true, CancellationToken.None);
                    _subscription = null;
                }

                if (_session != null)
                {
                    // Débrancher le KeepAlive avant de fermer
                    _session.KeepAlive -= OnKeepAlive;
                    await _session.CloseAsync();
                    _session.Dispose();
                    _session = null;
                }
            }
            catch
            {
                // Ignorer les erreurs de nettoyage —
                // la session est peut-être déjà morte
            }
        }
        // DÉCONNEXION MANUELLE

        public async Task DisconnectAsync()
        {
            Log($"[{_plc?.Name}] Déconnexion...");
            await CleanupSessionAsync();
            SetStatus(PlcStatus.Offline);
            Console.WriteLine(" Déconnecté.");
        }


        // STATUT + LOGGING

        private void SetStatus(PlcStatus status)
        {
            _currentStatus = status;

            var emoji = status switch
            {
                PlcStatus.Online => "OK",
                PlcStatus.Offline => "STOP",
                PlcStatus.Reconnecting => "PAUSE",
                _ => "STAND-BY"
            };

            Log($"[STATUS] {emoji} {_plc?.Name} — {status.ToString().ToUpper()}");
        }
        private void Log(string message, LogService.LogLevel level = LogService.LogLevel.INFO)
        {
            _ = _logger.LogAsync(level, message, _plc?.Name ?? "SYSTEM");
        }

        /*
        //QUAND y'AVAIS ConfigService
        private static void Log(string message)
        {
            // Format : [2026-08-10 14:32:01] message
            // Prépare le terrain pour le Fehlerfile (Phase 1b)
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
        }
        */

        // CONFIGURATION OPC-UA

        private static async Task<ApplicationConfiguration> BuildConfigAsync()
        {
            const string certStore =
                @"%CommonApplicationData%\OPC Foundation\CertificateStores";

            var config = new ApplicationConfiguration
            {
                ApplicationName = "OPC UA - TENNECO",
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
                    OperationTimeout = 60000,
                    MaxStringLength = 1048576,
                    MaxByteStringLength = 1048576,
                    MaxArrayLength = 65535,
                    MaxMessageSize = 4194304,
                },
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = $"{certStore}\\MachineDefault",
                        SubjectName = "OPC UA - JORDI-TENNECO"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = $"{certStore}\\UA Applications"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = $"{certStore}\\UA Certificate Authorities"
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = $"{certStore}\\RejectedCertificates"
                    },
                    AutoAcceptUntrustedCertificates = true,
                    AddAppCertToTrustedStore = true
                }
            };

            await config.ValidateAsync(ApplicationType.Client);

            config.CertificateValidator.CertificateValidation += (s, e) =>
            {
                e.Accept = true;
            };

            return config;
        }


        // IDISPOSABLE

        public void Dispose()
        {
            if (_disposed) return;
            _session?.Dispose();
            _subscription?.Dispose();
            _reconnectLock.Dispose();
            _disposed = true;
        }
    }
}




