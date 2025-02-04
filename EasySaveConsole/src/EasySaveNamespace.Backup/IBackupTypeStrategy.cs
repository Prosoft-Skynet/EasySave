namespace EasySaveConsole.EasySaveNamespace.Backup;

public interface IBackupTypeStrategy
{
    public void ExecuteBackupStrategy(string source, string target);
}
