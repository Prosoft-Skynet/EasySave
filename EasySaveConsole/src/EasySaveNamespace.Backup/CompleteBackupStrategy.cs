namespace EasySaveConsole.EasySaveNamespace.Backup;

/// <summary>
/// Implements the complete backup strategy.
/// </summary>
public class CompleteBackupStrategy : IBackupTypeStrategy
{
    /// <summary>
    /// Executes a complete backup.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="target">The target directory.</param>
    public void ExecuteBackupStrategy(string source, string target)
    {
        var backupService = new BackupService();
        backupService.TransferFiles(source, target);
    }
}