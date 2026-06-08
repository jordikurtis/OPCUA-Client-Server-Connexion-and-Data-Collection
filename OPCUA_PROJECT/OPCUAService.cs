using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

//using System.Text.Json; // using System.Text.Json;

namespace OPCUA_PROJECT
{
    public class OPCUAService : IOpcUaClient, IDisposable
    {
        /// Implémentation du client OPC UA.
        /// Gère connexion, session, subscription et monitoring pour UN PLC.
        /// Appelée en parallèle pour chaque PLC.

        
            private Session? _session;
            private Subscription? _subscription;
            private bool _disposed;
        public bool IsConnected => _session?.Connected ?? false;
        /*

        _session?.Connected – Safely checks if _session is not null; if it exists, it reads the Connected property.??
        false – If _session is null (so .Connected cannot be read), it defaults to false.
=> – This is a short-hand (expression-bodied property) for a read-only property; it directly returns the expression result.
         
      }*/

        // ── Connexion + Monitoring ────────────────────────────
        public async Task ConnectAndMonitorAsync(
                PlcDefinition plc,
                Func<MeasurementsData, Task> onDataReceived)
            {
                Console.WriteLine($"\n[{plc.Name}] Connexion à {plc.EndpointUrl}...");

                var config = await BuildConfigAsync();

                // Endpoint
                var endpointDesc = CoreClientUtils.SelectEndpoint(config, plc.EndpointUrl, useSecurity: false);
                var endpointConfig = EndpointConfiguration.Create(config);
                var endpoint = new ConfiguredEndpoint(null, endpointDesc, endpointConfig);

                // Session
                _session = await Session.Create(
                    config,
                    endpoint,
                    true,
                    $"Session_{plc.Name}",
                    60000,
                    new UserIdentity(new AnonymousIdentityToken()),
                    null);

                _session.KeepAlive += OnKeepAlive;

                Console.WriteLine($"[{plc.Name}]  Connecté — Session: {_session.SessionId}");

                // Subscription
                _subscription = new Subscription(_session.DefaultSubscription)
                {
                    PublishingInterval = 3000,
                    PublishingEnabled = true,
                    Priority = 0
                };

                _session.AddSubscription(_subscription);
                _subscription.Create();

            // Monitored Items
            //affichage du nom du noeud au lieu du NodeId brut :

            foreach (var nodeId in plc.NodesToMonitor)
                {
                var item = new MonitoredItem(_subscription.DefaultItem)
                {
                    StartNodeId = NodeId.Parse(nodeId),
                    AttributeId = Attributes.Value,
                    SamplingInterval = 0,
                    QueueSize = 10,
                    DiscardOldest = true,

                    DisplayName = $"{plc.Name}:{nodeId}",
                      // item.DisplayName = $"{plc.Name} | {node.Name} ({node.NodeId})"
                   // DisplayName = $"{plc.Name} | {node.Name} ({node.NodeId})"
                };

                    // Capture des variables pour le handler
                    var capturedPlcName = plc.Name;
                    var capturedNodeId = nodeId;

                    item.Notification += (mi, e) =>
                        HandleNotification(capturedPlcName, capturedNodeId, mi, e, onDataReceived);

                    _subscription.AddItem(item);
                    Console.WriteLine($"[{plc.Name}] Monitoring: {nodeId}");
                }

                _subscription.ApplyChanges();

                // Garder la session active
                await Task.Delay(Timeout.Infinite);
            }

            public async Task DisconnectAsync()
            {
                if (_subscription != null && _session != null)
                {
                    try
                    {
                        await _subscription.DeleteAsync(true, CancellationToken.None);
                        await _session.RemoveSubscriptionAsync(_subscription);
                    }
                    catch { }
                    _subscription = null;
                }

                if (_session?.Connected == true)
                {
                    try { await _session.CloseAsync(); }
                    catch { }
                }

                Console.WriteLine("🔌 Déconnecté.");
            }

            // ── Handlers ─────────────────────────────────────────
            private static void HandleNotification(
                string plcName,
                string nodeId,
                MonitoredItem item,
                MonitoredItemNotificationEventArgs e,
                Func<MeasurementsData, Task> onDataReceived)
            {
                foreach (var value in item.DequeueValues())
                {
                    var data = new MeasurementsData
                    {
                        PlcName = plcName,
                        NodeId = nodeId,
                        Value = value.Value,
                        Status = value.StatusCode.ToString(),
                        SourceTimestamp = value.SourceTimestamp
                    };

                    Console.WriteLine($"--- LIVE UPDATE ---");
                    Console.WriteLine(data);
                    Console.WriteLine();
                // LIGNE AJOUTER 
                Console.WriteLine($"Valeur    : {value.Value} {data.Unit}");

                // Appel asynchrone fire-and-forget
                _ = onDataReceived(data);
                }
            }

            private static void OnKeepAlive(ISession session, KeepAliveEventArgs e)
            {
                if (ServiceResult.IsBad(e.Status))
                    Console.WriteLine($" KeepAlive — Problème: {e.Status}");
            }

            // ── Configuration OPC UA ─────────────────────────────
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

            // ── IDisposable ──────────────────────────────────────
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        _session?.Dispose();
                        _subscription?.Dispose();
                    }
                    _disposed = true;
                }
            }
        }
    }

