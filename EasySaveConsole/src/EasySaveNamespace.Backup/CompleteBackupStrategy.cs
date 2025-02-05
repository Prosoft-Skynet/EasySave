namespace EasySaveConsole.EasySaveNamespace.Backup;

public class CompleteBackupStrategy : IBackupTypeStrategy
{
    /// <summary>
    /// Exécute une sauvegarde complète.
    /// </summary>
    /// <param name="source">Répertoire source.</param>
    /// <param name="target">Répertoire cible.</param>
    public void ExecuteBackupStrategy(string source, string target)
    {
        var backupService = new BackupService();
        backupService.TransferFiles(source, target);
    }
}