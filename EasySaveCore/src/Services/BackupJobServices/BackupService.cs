namespace EasySaveCore.src.Services.BackupJobServices;

using EasySaveCore.src.Models;
using EasySaveCore.src.Services;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;

/// <summary>
/// Manages backup jobs, including adding, executing, and restoring backups.
/// </summary>
public class BackupService
{
    IBackupTypeStrategy? backupStrategy = null;
    private List<BackupJobModel> backupJobs = new List<BackupJobModel>();
    private StateService _stateService = new StateService();
    private BusinessApplicationService _businessApplicationService = new BusinessApplicationService();
    public List<string> extensionsToEncrypt = new List<string> { ".txt", ".docx" }; // Extensions to be encrypted
    private readonly string _backupJobsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backupJobs.json");
    private bool _isPaused;
    private bool _isStopped;

    public BackupService()
    {
        LoadBackupJobs();
    }

    public void PauseBackup()
    {
        _isPaused = true;
    }

    public void ResumeBackup()
    {
        _isPaused = false;
    }

    public void StopBackup()
    {
        _isStopped = true;
        _isPaused = false;
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
    public List<BackupJobModel> LoadBackupJobs()
    {
        try
        {
            if (File.Exists(_backupJobsFilePath))
            {
                string json = File.ReadAllText(_backupJobsFilePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    List<BackupJobModel> BJobs = JsonSerializer.Deserialize<List<BackupJobModel>>(json);
                    backupJobs = BJobs ?? new List<BackupJobModel>();

                    foreach (var job in backupJobs)
                    {
                        backupStrategy = job.IsFullBackup ? new CompleteBackupStrategy() : new DifferentialBackupStrategy();
                    }
                }
            }
            else
            {
                backupJobs = new List<BackupJobModel>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            backupJobs = new List<BackupJobModel>();
        }

        return backupJobs;
    }

    /// <summary>
    /// Adds a new backup job.
    /// </summary>
    /// <param name="job">The backup job to add.</param>
    public void AddBackup(BackupJobModel job)
    {
        backupJobs.Add(job);
    }

    public void ExecuteJob(Guid jobId)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            _stateService.UpdateState(job, "Loading");
            Execute(job);

            _stateService.UpdateState(job, "Finished");
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
            _stateService.UpdateState(job, "Loading");
            Execute(job);
            EncryptFilesInDirectory(job.Target, getEncryptionKeyCallback);
            _stateService.UpdateState(job, "Finished");
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
                CryptoSoftService.Crypt(new string[] { file, encryptionKey });
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

                CryptoSoftService.Crypt(new string[] { file, decryptionKey });
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
            Restore(job);
            DecryptFilesInDirectory(job.Source, getDecryptionKeyCallback);

        }
    }

    /// <summary>
    /// Retrieves the list of backup jobs.
    /// </summary>
    /// <returns>The list of backup jobs.</returns>
    public List<BackupJobModel> GetBackupJobs()
    {
        return backupJobs;
    }

    public void TransferFiles(string source, string target, ObservableCollection<string> filesExceptions, Action checkForPauseAndStop)
    {
        var sourceDirectory = new DirectoryInfo(source);
        var targetDirectory = new DirectoryInfo(target);

        if (!targetDirectory.Exists)
        {
            CreateDirectories(target);
        }

        foreach (var file in sourceDirectory.GetFiles())
        {
            checkForPauseAndStop();

            if (!filesExceptions.Contains(file.FullName))
            {
                file.CopyTo(Path.Combine(target, file.Name), true);
            }
        }

        foreach (var directory in sourceDirectory.GetDirectories())
        {
            var targetSubDirPath = Path.Combine(target, directory.Name);
            TransferFiles(directory.FullName, targetSubDirPath, filesExceptions, checkForPauseAndStop);
        }
    }

    /// <summary>
    /// Creates the necessary directories.
    /// </summary>
    /// <param name="path">The path of the directory to create.</param>
    public void CreateDirectories(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void Restore(BackupJobModel backUpJob)
    {
        TransferFiles(backUpJob.Target, backUpJob.Source, _businessApplicationService.GetBusinessApplications(), CheckForPauseAndStop);
    }

    private void Execute(BackupJobModel backupJob)
    {
        if (backupJob.IsFullBackup)
            backupStrategy = new CompleteBackupStrategy();
        else
            backupStrategy = new DifferentialBackupStrategy();

        backupStrategy.ExecuteBackupStrategy(backupJob.Source, backupJob.Target, _businessApplicationService.GetBusinessApplications(), CheckForPauseAndStop);
    }

    private void CheckForPauseAndStop()
    {
        while (_isPaused) // Attente en cas de pause
        {
            Thread.Sleep(100);
        }

        if (_isStopped)
        {
            throw new OperationCanceledException("Backup stopped by user.");
        }
    }


    public void SetBackupStrategy(IBackupTypeStrategy strategy)
    {
        backupStrategy = strategy;
    }

    public void SetBackupStrategy(bool isFullBackub)
    {
        if (isFullBackub)
        {
            backupStrategy = new CompleteBackupStrategy();
        }
        else
        {
            backupStrategy = new DifferentialBackupStrategy();
        }
    }
}
