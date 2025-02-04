namespace EasySaveConsole.EasySaveNamespace.Backup;

public class CompleteBackupStrategy : IBackupTypeStrategy
{
    public void ExecuteBackupStrategy(string source, string target)
    {
        var backupService = new BackupService();
        backupService.TransferFiles(source, target);
    }
}
