namespace EasySaveCore.Backup;

using System;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;

/// <summary>
/// Represents a backup job with details such as name, source, target, and backup type.
/// </summary>
public class BackupJob
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public bool IsFullBackup { get; set; }
    public ObservableCollection<string> BusinessApplications { get; set; }

    [JsonIgnore]
    private IBackupTypeStrategy? backupStrategy;

    /// <summary>
    /// Constructeur vide requis pour la désérialisation JSON.
    /// </summary>
    public BackupJob()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the BackupJob class.
    /// </summary>
    /// <param name="name">The name of the backup job.</param>
    /// <param name="source">The source directory.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="isFullBackup">Indicates whether the backup is a full backup.</param>
    /// <param name="strategy">The backup strategy to use.</param>
    public BackupJob(string name, string source, string target, bool isFullBackup, IBackupTypeStrategy strategy)
    {
        Id = Guid.NewGuid();
        Name = name;
        Source = source;
        Target = target;
        IsFullBackup = isFullBackup;
        backupStrategy = strategy;
        BusinessApplications = new ObservableCollection<string>();
    }

    /// <summary>
    /// Définit la stratégie de sauvegarde après désérialisation.
    /// </summary>
    public void SetBackupStrategy(IBackupTypeStrategy strategy)
    {
        backupStrategy = strategy;
    }

    /// <summary>
    /// Executes the backup job.
    /// </summary>
    public void Execute()
    {
        if (backupStrategy == null)
        {
            throw new InvalidOperationException("Backup strategy is not set.");
        }
        backupStrategy.ExecuteBackupStrategy(Source, Target, BusinessApplications);
    }

    /// <summary>
    /// Restores the backed-up files.
    /// </summary>
    public void Restore()
    {
        var backupService = new BackupService();
        backupService.TransferFiles(Target, Source, BusinessApplications);
    }
}
