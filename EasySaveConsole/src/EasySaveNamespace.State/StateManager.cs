using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasySaveConsole.EasySaveNamespace.Backup;

namespace EasySaveConsole.EasySaveNamespace.State;

public class StateManager
{
    private string stateFilePath;
    private Dictionary<Guid, StateEntry> currentState;

    /// <summary>
    /// Initialise le gestionnaire d'état avec un chemin pour le fichier d'état.
    /// Le fichier sera créé dans le dossier 'temp' du projet.
    /// </summary>
    /// <param name="path">Chemin du fichier d'état. Par défaut, il sera situé dans le dossier 'temp'.</param>
    public StateManager(string path = null)
{
    ConfigureStatePath(path);

    currentState = new Dictionary<Guid, StateEntry>();
    LoadState();
}


    /// <summary>
    /// Met à jour l'état d'un job de sauvegarde.
    /// </summary>
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

    /// <summary>
    /// Configure le chemin du fichier d'état.
    /// </summary>
    public void ConfigureStatePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            stateFilePath = Path.Combine(projectDirectory, "temp", "state.json");
        }
        else
        {
            stateFilePath = path;
        }
    }

    /// <summary>
    /// Récupère l'état actuel d'un job donné.
    /// </summary>
    public StateEntry GetCurrentState(Guid jobId)
    {
        return currentState.ContainsKey(jobId) ? currentState[jobId] : null;
    }

    /// <summary>
    /// Sauvegarde l'état actuel dans le fichier d'état.
    /// </summary>
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

    /// <summary>
    /// Charge l'état depuis le fichier d'état, si disponible.
    /// </summary>
    private void LoadState()
        {
            if (File.Exists(stateFilePath))
            {
                var json = File.ReadAllText(stateFilePath);
                currentState = JsonSerializer.Deserialize<Dictionary<Guid, StateEntry>>(json) ?? new Dictionary<Guid, StateEntry>();
            }
    }
    public string GetStateFilePath()
{
    return stateFilePath;
}

}
