namespace EasySaveCore.Controller;

using System;
using System.Collections.Generic;
using EasySaveCore.Backup;
using EasySaveCore.Language;

/// <summary>
/// Singleton class that manages backup jobs and language settings.
/// </summary>
public class EasySave
{
    private static EasySave? instance;
    private BackupManager backupManager;
    private LanguageManager languageManager;

    /// <summary>
    /// Private constructor to initialize the BackupManager and LanguageManager.
    /// </summary>
    private EasySave()
    {
        backupManager = new BackupManager();
        languageManager = new LanguageManager(new EnLanguage());
    }

    /// <summary>
    /// Gets the singleton instance of EasySave.
    /// </summary>
    /// <returns>The singleton instance of EasySave.</returns>
    public static EasySave GetInstance()
    {
        if (instance == null)
        {
            instance = new EasySave();
        }
        return instance;
    }

    /// <summary>
    /// Executes a backup job specified by its identifier.
    /// </summary>
    /// <param name="jobId">The identifier of the backup job.</param>
    public void Run(Guid jobId, Func<string, string> getEncryptionKeyCallback)
    {
        backupManager.ExecuteJobinterface(jobId, getEncryptionKeyCallback);
    }

    /// <summary>
    /// Lists all available backup jobs.
    /// </summary>
    /// <returns>A list of backup jobs.</returns>
    public List<BackupJob> ListBackup()
    {
        return backupManager.GetBackupJobs();
    }

    /// <summary>
    /// Sets the current language of the application using the LanguageManager.
    /// </summary>
    /// <param name="language">The language instance to apply.</param>
    public void SetLanguage(Language language)
    {
        languageManager.SetLanguage(language);
    }

    /// <summary>
    /// Retrieves the translation for a specific key using the LanguageManager.
    /// </summary>
    /// <param name="key">The key for the desired translation.</param>
    /// <returns>
    /// The translation associated with the key if it exists.
    /// Returns "Key not found: {key}" if the key does not exist.
    /// </returns>
    public string GetText(string key)
    {
        return languageManager.GetText(key);
    }
}