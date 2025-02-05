using EasySaveConsole.EasySaveNamespace.Backup;

namespace EasySaveConsole.EasySaveNamespace;

public class BackupManager
{
    private List<BackupJob> backupJobs = new List<BackupJob>();

    public void AddBackup(BackupJob job)
    {
        backupJobs.Add(job);
    }
    public void ExecuteJob(Guid jobId)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            job.Execute();

        }
    }

    public void ExecuteJobsSequentially(List<Guid> jobIds)
    {
        foreach (var jobId in jobIds)
        {
            ExecuteJob(jobId);
        }
    }

    public void RestoreJob(Guid jobId)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            job.Restore();
        }
    }

    public List<BackupJob> GetBackupJobs()
    {
        return backupJobs;
    }
}