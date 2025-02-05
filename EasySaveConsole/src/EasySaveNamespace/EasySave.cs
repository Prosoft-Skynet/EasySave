namespace EasySaveConsole.EasySaveNamespace;

using System;
using System.Collections.Generic;
using EasySaveConsole.EasySaveNamespace.Backup;
using EasySaveConsole.EasySaveNamespace.Language;

public class EasySave
{
    private static EasySave? instance;
    private BackupManager backupManager;
    private LanguageManager languageManager;

    private EasySave()
    {
        backupManager = new BackupManager();
        languageManager = new LanguageManager(new EnLanguage());
    }

    public static EasySave GetInstance()
    {
        if (instance == null)
        {
            instance = new EasySave();
        }
        return instance;
    }

    /// <summary>
    /// Exécute un travail de sauvegarde spécifié par son identifiant.
    /// </summary>
    /// <param name="jobId">Identifiant du travail de sauvegarde.</param>
    public void Run(Guid jobId)
    {
        backupManager.ExecuteJob(jobId);
    }

    /// <summary>
    /// Liste tous les travaux de sauvegarde disponibles.
    /// </summary>
    /// <returns>Liste des travaux de sauvegarde.</returns>
    public List<BackupJob> ListBackup()
    {
        return backupManager.GetBackupJobs();
    }

    /// <summary>
    /// Définit la langue actuelle de l'application en utilisant le LanguageManager.
    /// </summary>
    /// <param name="language">Instance de la langue à appliquer.</param>
    public void SetLanguage(Language.Language language)
    {
        languageManager.SetLanguage(language);
    }

    /// <summary>
    /// Récupère la traduction d'une clé spécifique en utilisant le LanguageManager.
    /// </summary>
    /// <param name="key">Clé de la traduction recherchée.</param>
    /// <returns>
    /// La traduction associée à la clé si elle existe.
    /// Retourne "Clé introuvable: {key}" si la clé n'existe pas.
    /// </returns>
    public string GetText(string key)
    {
        return languageManager.GetText(key);
    }

}
