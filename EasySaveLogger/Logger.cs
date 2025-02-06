using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySaveLogger
{
    public class Logger
    {
        private readonly string _logDirectory;

        public Logger()
        {
            this._logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            
            
            this.ConfigureLogPath();

            // Création du dossier si nécessaire
            Directory.CreateDirectory(_logDirectory);
        }

        private void ConfigureLogPath()
        {
            if (this._logDirectory != null)
            {
                if (!Directory.Exists(this._logDirectory))
                {
                    Directory.CreateDirectory(this._logDirectory);
                }
            }
        }

        public void Log(string backupName, string sourcePath, string destinationPath, long transferTimeMs)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string logFilePath = Path.Combine(_logDirectory, $"{date}.json");

            List<LogEntry> logs = new();

            // Lire le fichier existant s'il existe
            if (File.Exists(logFilePath))
            {
                string existingJson = File.ReadAllText(logFilePath);
                logs = JsonSerializer.Deserialize<List<LogEntry>>(existingJson) ?? new List<LogEntry>();
            }

            string[] files = Directory.GetFiles(sourcePath);
            long fileSize = 0;
            foreach (string file in files) {
                fileSize += new FileInfo(file).Length;
            }



            // Ajouter une nouvelle entrée
            logs.Add(new LogEntry
            (
                DateTime.Now.ToString("o"), // Format ISO 8601
                backupName,
                sourcePath,
                destinationPath,
                fileSize,
                transferTimeMs
            ));

            // Sauvegarde en JSON
            string json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(logFilePath, json);
        }
    }
}
