using EasySaveConsole.EasySaveNamespace.Backup;
using EasySaveConsole.EasySaveNamespace.State;

namespace EasySaveConsole.EasySaveNamespace;

public class BackupManager
{
    private List<BackupJob> backupJobs = new List<BackupJob>();
    private StateManager stateManager = new StateManager();

    public void AddBackup(BackupJob job)
    {
        backupJobs.Add(job);
    }
   public void ExecuteJob(Guid jobId)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            stateManager.UpdateState(job, "Actif", 0, 0, 0, 0, 0, job.Source, job.Target);
            job.Execute();
            stateManager.UpdateState(job, "Terminé", 100, 100, 1.0f, 0, 0, "", "");
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