namespace EasySaveConsole.EasySaveNamespace;

using EasySaveConsole.EasySaveNamespace.Backup;
using EasySaveConsole.EasySaveNamespace.State;

/// <summary>
/// Manages backup jobs, including adding, executing, and restoring backups.
/// </summary>
public class BackupManager
{
    private List<BackupJob> backupJobs = new List<BackupJob>();
    private StateManager stateManager = new StateManager();

    /// <summary>
    /// Adds a new backup job.
    /// </summary>
    /// <param name="job">The backup job to add.</param>
    public void AddBackup(BackupJob job)
    {
        backupJobs.Add(job);
    }

    /// <summary>
    /// Executes a backup job specified by its identifier.
    /// </summary>
    /// <param name="jobId">The identifier of the backup job.</param>
    public void ExecuteJob(Guid jobId)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            stateManager.UpdateState(job, "In progress", 0, 0, 0, 0, 0, job.Source, job.Target);
            job.Execute();
            stateManager.UpdateState(job, "Finish", 100, 100, 1.0f, 0, 0, "", "");
        }
    }

    /// <summary>
    /// Executes a list of backup jobs sequentially.
    /// </summary>
    /// <param name="jobIds">The list of backup job identifiers.</param>
    public void ExecuteJobsSequentially(List<Guid> jobIds)
    {
        foreach (var jobId in jobIds)
        {
            ExecuteJob(jobId);
        }
    }

    /// <summary>
    /// Restores a backup job specified by its identifier.
    /// </summary>
    /// <param name="jobId">The identifier of the backup job.</param>
    public void RestoreJob(Guid jobId)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            job.Restore();
        }
    }

    /// <summary>
    /// Retrieves the list of backup jobs.
    /// </summary>
    /// <returns>The list of backup jobs.</returns>
    public List<BackupJob> GetBackupJobs()
    {
        return backupJobs;
    }
}