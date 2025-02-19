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
    private readonly string _backupJobsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backupJobs.json");
    private bool _isPaused;
    private bool _isStopped;

    private readonly string ExtensionsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extensions.txt");
    public List<string> extensionsToEncrypt { get; private set; } = new List<string>();


    public BackupService()
    {
        LoadBackupJobs();
        LoadExtensionsToEncrypt();


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
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"Directory does not exist: {directoryPath}");
            return;
        }

        var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            Console.WriteLine($"No files found in directory: {directoryPath}. Skipping encryption.");
            return;
        }

        Console.WriteLine($"Files found in {directoryPath}:");
        foreach (var file in files)
        {
            Console.WriteLine($"  - {file}");
        }

        foreach (var file in files)
        {
            LoadExtensionsToEncrypt();
            var fileExtension = Path.GetExtension(file).ToLowerInvariant().Trim();

            if (extensionsToEncrypt.Contains(fileExtension))
            {
                string encryptionKey = getEncryptionKeyCallback(Path.GetFileName(file));

                if (string.IsNullOrEmpty(encryptionKey))
                {
                    Console.WriteLine($"Encryption key is empty for file {file}. Skipping.");
                    continue;
                }

                Console.WriteLine($"Encrypting file: {file}");
                CryptoSoftService.Crypt(new string[] { file, encryptionKey });
            }
            else
            {
                Console.WriteLine($"File {file} has an unsupported extension. Skipping.");
            }
        }
    }


    private void DecryptFilesInDirectory(string directoryPath, Func<string, string> getDecryptionKeyCallback)
    {
        if (!Directory.Exists(directoryPath)) return;

        var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            Console.WriteLine($"No files found in directory: {directoryPath}. Skipping decryption.");
            return;
        }

        foreach (var file in files)
        {
            LoadExtensionsToEncrypt();
            var fileExtension = Path.GetExtension(file).ToLowerInvariant().Trim();

            if (extensionsToEncrypt.Contains(fileExtension))

                if (extensionsToEncrypt.Contains(Path.GetExtension(file)))
                {
                    string decryptionKey = getDecryptionKeyCallback(Path.GetFileName(file));

                    if (string.IsNullOrEmpty(decryptionKey))
                    {
                        continue;
                    }

                    Console.WriteLine($"Decrypting file: {file}");
                    CryptoSoftService.Crypt(new string[] { file, decryptionKey });
                }
                else
                {
                    Console.WriteLine($"File {file} has an unsupported extension. Skipping.");
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

    public List<string> LoadExtensionsToEncrypt()
    {
        if (File.Exists(ExtensionsFilePath))
        {
            extensionsToEncrypt = File.ReadAllLines(ExtensionsFilePath).ToList();
        }
        return extensionsToEncrypt;
    }

    public void AddExtensionToEncrypt(string extension)
    {
        if (!extensionsToEncrypt.Contains(extension))
        {
            extensionsToEncrypt.Add(extension);
            SaveExtensionsToEncrypt();
        }
    }

    public void RemoveExtensionToEncrypt(string extension)
    {
        if (extensionsToEncrypt.Contains(extension))
        {
            extensionsToEncrypt.Remove(extension);
            SaveExtensionsToEncrypt();
        }
    }

    private void SaveExtensionsToEncrypt()
    {
        File.WriteAllLines(ExtensionsFilePath, extensionsToEncrypt);
    }
}
