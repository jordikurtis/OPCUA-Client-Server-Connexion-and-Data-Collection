using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OPCUA_PROJECT
{
    /// <summary>
    /// Gère deux types de logs :
    /// - app_YYYY-MM-DD.log  → tout ce qui se passe (connexion, monitoring, DB)
    /// - errors_YYYY-MM-DD.csv → uniquement les erreurs (pour la maintenance)
    /// Rotation automatique par jour.
    /// Thread-safe via SemaphoreSlim.
    /// </summary>
    public class LogService
    {
        // Dossier des logs
        private readonly string _logDirectory ;

        // SemaphoreSlim : évite les conflits d'écriture
        // entre plusieurs PLCs qui écrivent en parallèle 
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        // Niveaux de log 
        public enum LogLevel
        {
            INFO,
            WARNING,
            ERROR
        }

        public LogService(string logDirectory = "Logs") //"H:\\Jordan Wilfried Werksstudent\\LogService"
        {
            _logDirectory = logDirectory;

            // Créer le dossier si inexistant
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
                Console.WriteLine($"[LogService] Dossier créé : {_logDirectory}");
            }

            // Créer le fichier CSV avec en-tête si inexistant
            EnsureCsvHeaderAsync().GetAwaiter().GetResult();
        }

        
        // MÉTHODE PRINCIPALE — écrire un log
      
        public async Task LogAsync(
            LogLevel level,
            string message,
            string plcName = "SYSTEM")
        {
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyy-MM-dd HH:mm:ss");
            var date = now.ToString("yyyy-MM-dd");

            // Format de la ligne pour le fichier .log
            var logLine = $"[{timestamp}] [{level,-7}] [{plcName,-20}] {message}";

            // Afficher aussi dans la console
            Console.WriteLine(logLine);

            await _lock.WaitAsync();
            try
            {
                // Écrire dans app_YYYY-MM-DD.log 
                var appLogPath = Path.Combine(_logDirectory, $"app_{date}.log");
                await File.AppendAllTextAsync(appLogPath, logLine + Environment.NewLine);

                //  Si c'est une erreur -> écrire aussi dans errors.csv 
                if (level == LogLevel.ERROR || level == LogLevel.WARNING)
                {
                    var csvPath = Path.Combine(_logDirectory, $"errors_{date}.csv");
                    var csvLine = $"{timestamp},{level},{plcName},\"{message}\"";
                    await File.AppendAllTextAsync(csvPath, csvLine + Environment.NewLine);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        // Raccourcis 
        public Task InfoAsync(string message, string plcName = "SYSTEM")
            => LogAsync(LogLevel.INFO, message, plcName);

        public Task WarningAsync(string message, string plcName = "SYSTEM")
            => LogAsync(LogLevel.WARNING, message, plcName);

        public Task ErrorAsync(string message, string plcName = "SYSTEM")
            => LogAsync(LogLevel.ERROR, message, plcName);

        
        // EN-TÊTE CSV
        // Créé une seule fois si le fichier n'existe pas
       
        private async Task EnsureCsvHeaderAsync()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var csvPath = Path.Combine(_logDirectory, $"errors_{date}.csv");

            if (!File.Exists(csvPath))
            {
                await File.WriteAllTextAsync(
                    csvPath,
                    "timestamp,level,plc_name,message" + Environment.NewLine);
            }
        }
    }
}


