using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;


namespace OPCUA_PROJECT
{
        public class ConfigServiceJSON
        {
            private readonly string _configPath;

            public ConfigServiceJSON(string configPath = "PROSY_JSON.json") //  OPCUA_NodeId_Config.json
        {
                _configPath = configPath;
            }

            public AppConfig LoadConfig()
            {
                if (!File.Exists(_configPath))
                {
                    Console.WriteLine($"[Config] Fichier '{_configPath}' introuvable.");
                    return new AppConfig();
                }

                try
                {
                    var json = File.ReadAllText(_configPath);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var config = JsonSerializer.Deserialize<AppConfig>(json, options)
                                 ?? new AppConfig();

                    // Filtrage léger (sans logique métier)
                    config.Plcs = config.Plcs
                        .Where(plc => plc.Enabled)
                        .Select(plc =>
                        {
                            plc.Nodes = plc.Nodes
                                .Where(n => n.Variables.Any(v => v.Enabled))
                                .Select(n =>
                                {
                                    n.Variables = n.Variables
                                        .Where(v => v.Enabled)
                                        .ToList();
                                    return n;
                                })
                                .ToList();
                            return plc;
                        })
                        .ToList();

                    Console.WriteLine($"[Config] {config.Plcs.Count} PLC(s) actif(s) chargé(s).");

                    foreach (var plc in config.Plcs)
                    {
                        Console.WriteLine($"[Config] → {plc.Name} ({plc.Nodes.Count} noeud(s))");
                    }

                    return config;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Config] Erreur lecture JSON: {ex.Message}");
                    return new AppConfig();
                }
            }
        }
    }
    /*
     Charge la configuration des PLCs depuis un fichier JSON.
    /// Si le fichier n'existe pas → crée un fichier exemple automatiquement.

     
     */
    /*
    public class ConfigServiceJSON
    {

        /// <summary>
        /// Charge la configuration des PLCs depuis un fichier JSON.
        /// Si le fichier n'existe pas → crée un fichier exemple automatiquement.
        /// </summary>
        private readonly string _configPath;

        public ConfigServiceJSON(string configPath = "OPCUA_NodeId_Config.json")
        {
            _configPath = configPath;
        }

        public List<PlcDefinition> LoadPlcs()
        {
            // Fichier introuvable → créer un exemple
            if (!File.Exists(_configPath))
            {
                Console.WriteLine($"[Config] Fichier '{_configPath}' introuvable.");
               
                return new List<PlcDefinition>();
            }

            try
            {
                var json = File.ReadAllText(_configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // endpointUrl = EndpointUrl 
                };

                var configFile = JsonSerializer.Deserialize<PlcConfigFile>(json, options);
                var plcs = configFile?.Plcs ?? new List<PlcDefinition>();

                // Filtrer les PLCs désactivés
                var activePlcs = plcs.Where(p => p.Enabled).ToList();

                Console.WriteLine($"[Config] {plcs.Count} PLC(s) trouvé(s)" +
                                  $" — {activePlcs.Count} actif(s)");

                foreach (var plc in activePlcs)
                    Console.WriteLine($"[Config]    → {plc.Name} " +
                                      $"({plc.Nodes.Count} noeud(s))");

                return activePlcs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Config] Erreur lecture JSON: {ex.Message}");
                return new List<PlcDefinition>();
            }
        }





    }
    




        /*public readonly string _configPath;

        public ConfigService(string configPath = "OPCUA_NodeId_Config.json")
        {
            _configpath = configPath;
        }

        public List <PlcDefinition> LoadPlcs()
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"[Config] Fichier '{configPath}' introuvalbe ");
                CreatExempleConfig();
                Console.WriteLine($"[Config] Fichier exemple crée -> modifie et relance ");

                return new List<PlcsDefinition>();

            }


            try {

                var json = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // nicht buchstaben Case-sensitive
                };


                var configFile = JsonSerializer.Deserializer<PlcConfigFile>(json, options);
                var plcs = configFile?.Plcs ?? new List<PlcDefinition>();

                //Filtre les Plcs désactivés 
                var activePlc = plcs.Where(plc => p.Enabled).ToList();

                Console.WriteLine($"[Config ] {plcs.Count} PLC(s) trouvé(s) " + $"{activePlcs.Count} actif(s) ");

                foreach (var plc in activePlcs)
                {
                    Console.WriteLine($"[conig] -> {plc.Name}" + $"({plc.Nodes.Count} noeuds(s)"));

                    return activePlc;
                }
            } catch (Exception ex) {

                Console.WriteLine($"[Config] Erreur lecture JSON: {ex.Message}");
                return new List<PlcDefinition>();

            }


        }

        private void CreatExempleConfig() {

            var exemple = new PlcConfigFile
            {
                Plcs = new PlcsConfigFile
                {
                    Plcs = new List<PlcsDefinition>
                {
                    new PlcsDefinition
                    {
                        /*
                         donne un format plausible ,  en fonction du PLC(machine en question )
                         
                         
                    }
                }
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_configPath, JsonSerializer.Serialize(example, options));


        }
*/




