namespace EasySaveCore.Controller;

using EasySaveCore.Backup;
using EasySaveCore.CryptoSoft;
using EasySaveCore.State;

/// <summary>
/// Manages backup jobs, including adding, executing, and restoring backups.
/// </summary>
public class BackupManager
{
    private List<BackupJob> backupJobs = new List<BackupJob>();
    private StateManager stateManager = new StateManager();
    private List<string> extensionsToEncrypt = new List<string> { ".txt", ".docx" }; // Extensions to be encrypted

    /// <summary>
    /// Adds a new backup job.
    /// </summary>
    /// <param name="job">The backup job to add.</param>
    public void AddBackup(BackupJob job)
    {
        backupJobs.Add(job);

    }

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
    /// Executes a backup job specified by its identifier.
    /// </summary>
    /// <param name="jobId">The identifier of the backup job.</param>
    public void ExecuteJobinterface(Guid jobId, Func<string, string> getEncryptionKeyCallback)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            stateManager.UpdateState(job, "In progress", 0, 0, 0, 0, 0, job.Source, job.Target);
            job.Execute();

            EncryptFilesInDirectory(job.Target, getEncryptionKeyCallback);

            stateManager.UpdateState(job, "Finish", 100, 100, 1.0f, 0, 0, "", "");
        }
    }
    private void EncryptFilesInDirectory(string directoryPath, Func<string, string> getEncryptionKeyCallback)
    {
        if (!Directory.Exists(directoryPath)) return;

        foreach (var file in Directory.GetFiles(directoryPath))
        {
            if (extensionsToEncrypt.Contains(Path.GetExtension(file)))
            {
                string encryptionKey = getEncryptionKeyCallback(Path.GetFileName(file));

                if (string.IsNullOrEmpty(encryptionKey))
                {
                    continue; 
                }
                CryptoSoft.Crypt(new string[] { file, encryptionKey });
            }
        }
    }

    /// <summary>
    /// Executes a list of backup jobs sequentially.
    /// </summary>
    /// <param name="jobIds">The list of backup job identifiers.</param>
    public void ExecuteJobsSequentially(List<Guid> jobIds, Func<string, string> getEncryptionKeyCallback)
    {
        foreach (var jobId in jobIds)
        {
            ExecuteJobinterface(jobId, getEncryptionKeyCallback);
        }
    }

    /// <summary>
    /// Restores a backup job specified by its identifier.
    /// </summary>
    /// <param name="jobId">The identifier of the backup job.</param>
    public void RestoreJob(Guid jobId, Func<string, string> getEncryptionKeyCallback)
    {

        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            EncryptFilesInDirectory(job.Target, getEncryptionKeyCallback);
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


