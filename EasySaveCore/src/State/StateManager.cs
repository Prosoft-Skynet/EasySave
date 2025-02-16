namespace EasySaveCore.State;

using System.Text.Json;
using EasySaveCore.Backup;

public class StateManager
{
    private string stateFilePath = string.Empty;
    private Dictionary<Guid, StateEntry> currentState;

    /// <summary>
    ///Initializes the state manager with a path for the state file.
    /// The file will be created in the project's 'temp' folder.
    /// </summary>
    /// <param name="path">Chemin du fichier d'état. Par défaut, il sera situé dans le dossier 'temp'.</param>
    public StateManager(string path = null!)
    {
        ConfigureStatePath(path);

        currentState = new Dictionary<Guid, StateEntry>();
        LoadState();
    }

    /// <summary>
    /// Update the state of a backup job
    /// </summary>
    /// <param name="job">Backup job to update</param>
    /// <param name="status">Current status of the job</param>
    /// <param name="filesTotal">Total number of files to be backed up</param>
    /// <param name="sizeTotal">Total size of the files to be backed up (in bytes)</param>
    /// <param name="progress">Job progress, between 0 (no progress) and 1 (fully completed)</param>
    /// <param name="remainingFiles">Number of remaining files to be processed</param>
    /// <param name="remainingSize">Size of the remaining files to be processed (in bytes)</param>
    /// <param name="currentSource">Current source of the backup job (source directory or file)</param>
    /// <param name="currentTarget">Current destination of the backup (target directory)</param>
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
    /// Configure the state file path.
    /// </summary>
    /// <param name="path">Path of the state file</param>
    public void ConfigureStatePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            stateFilePath = Path.Combine(projectDirectory, "State", "state.json");
        }
        else
        {
            stateFilePath = path;
        }
    }

    /// <summary>
    /// Retrieve the current state of a given job.
    /// </summary>
    /// <param name="jobId">Identifier of the job</param>
    public StateEntry GetCurrentState(Guid jobId)
    {
        return currentState.ContainsKey(jobId) ? currentState[jobId] : null!;
    }

    /// <summary>
    /// Save the current state to the state file.
    /// </summary>
    private void SaveState()
    {
        string directory = Path.GetDirectoryName(stateFilePath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(currentState, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(stateFilePath, json);
    }

    /// <summary>
    /// Load the current state from the state file.
    /// </summary>
    private void LoadState()
    {
        if (File.Exists(stateFilePath))
        {
            var json = File.ReadAllText(stateFilePath);
            currentState = JsonSerializer.Deserialize<Dictionary<Guid, StateEntry>>(json) ?? new Dictionary<Guid, StateEntry>();
        }
    }

    /// <summary>
    /// Retrieve the path of the state file.
    /// </summary>
    public string GetStateFilePath()
    {
        return stateFilePath;
    }
}
