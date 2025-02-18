namespace EasySaveCore.src.Services;

using System.Text.Json;
using EasySaveCore.src.Models;

public class StateService
{
    private string stateFilePath = string.Empty;
    private Dictionary<Guid, StateModel> currentState;

    /// <summary>
    ///Initializes the state manager with a path for the state file.
    /// The file will be created in the project's 'temp' folder.
    /// </summary>
    /// <param name="path">Chemin du fichier d'état. Par défaut, il sera situé dans le dossier 'temp'.</param>
    public StateService(string path = null!)
    {
        ConfigureStatePath(path);

        currentState = new Dictionary<Guid, StateModel>();
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
    public void UpdateState(BackupJobModel job, string state)
    {
        var stateModel = new StateModel();

        switch(state){
            case "Finished":
                stateModel = GenerateFinishedStateModel(job);
                break;
            case "Loading":
                stateModel = GenerateLoadingStateModel(job);
                break;
            default:
                stateModel = new StateModel();
                break;
        }
            
        
        
        currentState[job.Id] = stateModel;
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



    public StateModel GenerateLoadingStateModel(BackupJobModel job)
    {
        var loadingState = new StateModel
        {
            JobName = job.Name,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Status = "In progress",
            FilesTotal = 0,
            SizeTotal = 0,
            Progress = 0,
            RemainingFiles = 0,
            RemainingSize = 0,
            CurrentSource = job.Source,
            CurrentTarget = job.Target
        };

        return loadingState;
    }

    public StateModel GenerateFinishedStateModel(BackupJobModel job)
    {
        var loadingState = new StateModel
        {
            JobName = job.Name,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Status = "Finish",
            FilesTotal = 100,
            SizeTotal = 100,
            Progress = 1.0f,
            RemainingFiles = 0,
            RemainingSize = 0,
            CurrentSource = "currentSource",
            CurrentTarget = "currentTarget"
        };

        return loadingState;
    }

    /// <summary>
    /// Retrieve the current state of a given job.
    /// </summary>
    /// <param name="jobId">Identifier of the job</param>
    public StateModel GetCurrentState(Guid jobId)
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
            currentState = JsonSerializer.Deserialize<Dictionary<Guid, StateModel>>(json) ?? new Dictionary<Guid, StateModel>();
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
