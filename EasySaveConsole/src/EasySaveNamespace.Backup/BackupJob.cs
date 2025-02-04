namespace EasySaveConsole.EasySaveNamespace.Backup;

using System;

public class BackupJob
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Source { get; private set; }
    public string Target { get; private set; }
    public bool IsFullBackup { get; private set; }
    private IBackupTypeStrategy backupStrategy;

    public BackupJob(string name, string source, string target, bool isFullBackup, IBackupTypeStrategy strategy)
    {
        Id = Guid.NewGuid();
        Name = name;
        Source = source;
        Target = target;
        IsFullBackup = isFullBackup;
        backupStrategy = strategy;
    }

    public void Execute()
    {
        backupStrategy.ExecuteBackupStrategy(Source, Target);
    }

    public void Restore()
    {
        var backupService = new BackupService();
        backupService.TransferFiles(Target, Source);
    }
}
