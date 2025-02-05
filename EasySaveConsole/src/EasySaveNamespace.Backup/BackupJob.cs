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

    /// <summary>
    /// Initialise une nouvelle instance de la classe BackupJob.
    /// </summary>
    /// <param name="name">Nom du travail de sauvegarde.</param>
    /// <param name="source">Répertoire source.</param>
    /// <param name="target">Répertoire cible.</param>
    /// <param name="isFullBackup">Indique si la sauvegarde est complète.</param>
    /// <param name="strategy">Stratégie de sauvegarde à utiliser.</param>
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
    /// Exécute le travail de sauvegarde.
    /// </summary>
    public void Execute()
    {
        backupStrategy.ExecuteBackupStrategy(Source, Target);
    }

    /// <summary>
    /// Restaure les fichiers sauvegardés.
    /// </summary>
    public void Restore()
    {
        var backupService = new BackupService();
        backupService.TransferFiles(Target, Source);
    }
}