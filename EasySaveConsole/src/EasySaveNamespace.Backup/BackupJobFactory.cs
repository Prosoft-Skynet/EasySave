namespace EasySaveConsole.EasySaveNamespace.Backup;

using System.Collections.Generic;
using System.Threading.Tasks;

public class BackupJobFactory
{
    public void CreateBackupJobsInParallel(List<BackupJob> jobs)
    {
        List<Task> tasks = new List<Task>();

        foreach (var job in jobs)
        {
            tasks.Add(Task.Run(() => job.Execute()));
        }

        Task.WaitAll(tasks.ToArray());
    }
}
