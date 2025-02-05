using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasySaveConsole.EasySaveNamespace.Backup;

namespace EasySaveConsole.EasySaveNamespace.State
{
    public class StateManager
    {
        private string stateFilePath;
        private Dictionary<Guid, StateEntry> currentState;

        public StateManager(string path = "C:/temp/state.json")  
        {
            stateFilePath = path;
            currentState = new Dictionary<Guid, StateEntry>();
            LoadState();
        }

    public void UpdateState(BackupJob job, string status, int filesTotal, int sizeTotal, float progress, int remainingFiles, int remainingSize, string currentSource, string currentTarget)
        {
            var entry = new StateEntry
            {
                JobName = job.Name,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = status,
                FilesTotal = filesTotal,
                SizeTotal = sizeTotal,
                Progress = progress,
                RemainingFiles = remainingFiles,
                RemainingSize = remainingSize,
                CurrentSource = currentSource,
                CurrentTarget = currentTarget
            };

            currentState[job.Id] = entry;
            SaveState();
        }

        public void ConfigureStatePath(string path)
        {
            stateFilePath = path;
        }

        public StateEntry GetCurrentState(Guid jobId)
        {
            return currentState.ContainsKey(jobId) ? currentState[jobId] : null;
        }

        private void SaveState()
        {
            string directory = Path.GetDirectoryName(stateFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(currentState, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(stateFilePath, json);
        }


        private void LoadState()
        {
            if (File.Exists(stateFilePath))
            {
                var json = File.ReadAllText(stateFilePath);
                currentState = JsonSerializer.Deserialize<Dictionary<Guid, StateEntry>>(json) ?? new Dictionary<Guid, StateEntry>();
            }
        }
    }
}
