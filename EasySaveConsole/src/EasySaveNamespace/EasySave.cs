namespace EasySaveConsole.EasySaveNamespace;

using System;
using System.Collections.Generic;
using EasySaveConsole.EasySaveNamespace.Backup;

public class EasySave
{
    private static EasySave? instance;
    private BackupManager backupManager;

    private EasySave()
    {
        backupManager = new BackupManager();
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
}