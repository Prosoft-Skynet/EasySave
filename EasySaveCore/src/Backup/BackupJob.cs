namespace EasySaveCore.Backup;

using System;

/// <summary>
/// Represents a backup job with details such as name, source, target, and backup type.
/// </summary>
public class BackupJob
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Source { get; private set; }
    public string Target { get; private set; }
    public bool IsFullBackup { get; private set; }
    private IBackupTypeStrategy backupStrategy;

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
    }

    /// <summary>
    /// Executes the backup job.
    /// </summary>
    public void Execute()
    {
        backupStrategy.ExecuteBackupStrategy(Source, Target);
    }

    /// <summary>
    /// Restores the backed-up files.
    /// </summary>
    public void Restore()
    {
        var backupService = new BackupService();
        backupService.TransferFiles(Target, Source);
    }
}