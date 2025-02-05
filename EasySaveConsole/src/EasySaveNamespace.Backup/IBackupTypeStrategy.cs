namespace EasySaveConsole.EasySaveNamespace.Backup;

public interface IBackupTypeStrategy
{
    /// <summary>
    /// Exécute la stratégie de sauvegarde.
    /// </summary>
    /// <param name="source">Répertoire source.</param>
    /// <param name="target">Répertoire cible.</param>
    public void ExecuteBackupStrategy(string source, string target);
}