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

    public void Run(Guid jobId)
    {
        backupManager.ExecuteJob(jobId);
    }

    public List<BackupJob> ListBackup()
    {
        return backupManager.GetBackupJobs();
    }
    public void SetLanguage(Language.Language language) 
    {
        languageManager.SetLanguage(language);
    }

    public string GetText(string key)
    {
        return languageManager.GetText(key);
    }

}
