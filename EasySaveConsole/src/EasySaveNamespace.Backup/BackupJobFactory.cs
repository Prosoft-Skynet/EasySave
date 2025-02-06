namespace EasySaveConsole.EasySaveNamespace.Backup;

using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Factory class responsible for creating and executing backup jobs in parallel.
/// </summary>
public class BackupJobFactory
{
    /// <summary>
    /// Creates and executes backup jobs in parallel.
    /// </summary>
    /// <param name="jobs">List of backup jobs to execute.</param>
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