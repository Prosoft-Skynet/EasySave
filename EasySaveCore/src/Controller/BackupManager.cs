namespace EasySaveCore.Controller;

using EasySaveCore.Backup;
using EasySaveCore.CryptoSoft;
using EasySaveCore.State;
using System.Text.Json;

/// <summary>
/// Manages backup jobs, including adding, executing, and restoring backups.
/// </summary>
public class BackupManager
{
    private List<BackupJob> backupJobs = new List<BackupJob>();
    private StateManager stateManager = new StateManager();
    public List<string> extensionsToEncrypt = new List<string> { ".txt", ".docx" }; // Extensions to be encrypted
    private readonly string _backupJobsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backupJobs.json");

    public BackupManager()
    {
        LoadBackupJobs();
    }

    /// <summary>
    /// Saves the backup jobs to a JSON file.
    /// </summary>
    public void SaveBackupJobs()
    {
        string json = JsonSerializer.Serialize(backupJobs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_backupJobsFilePath, json);
    }

    /// <summary>
    /// Loads the backup jobs from a JSON file.
    /// </summary>
    public List<BackupJob> LoadBackupJobs()
    {
        try
        {
            if (File.Exists(_backupJobsFilePath))
            {
                string json = File.ReadAllText(_backupJobsFilePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    backupJobs = JsonSerializer.Deserialize<List<BackupJob>>(json) ?? new List<BackupJob>();

                    foreach (var job in backupJobs)
                    {
                        job.SetBackupStrategy(job.IsFullBackup ? new CompleteBackupStrategy() : new DifferentialBackupStrategy());
                    }
                }
            }
            else
            {
                backupJobs = new List<BackupJob>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            backupJobs = new List<BackupJob>();
        }

        return backupJobs;
    }

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

    private void DecryptFilesInDirectory(string directoryPath, Func<string, string> getDecryptionKeyCallback)
    {
        if (!Directory.Exists(directoryPath)) return;

        foreach (var file in Directory.GetFiles(directoryPath))
        {
            if (extensionsToEncrypt.Contains(Path.GetExtension(file)))
            {
                string decryptionKey = getDecryptionKeyCallback(Path.GetFileName(file));

                if (string.IsNullOrEmpty(decryptionKey))
                {
                    continue;
                }

                CryptoSoft.Crypt(new string[] { file, decryptionKey });
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
    public void RestoreJob(Guid jobId, Func<string, string> getDecryptionKeyCallback)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            job.Restore();
            DecryptFilesInDirectory(job.Source, getDecryptionKeyCallback);

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
