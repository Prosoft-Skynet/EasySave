namespace EasySaveConsole.EasySaveNamespace.Backup;

using System.Collections.Generic;
using System.Threading.Tasks;

public class BackupJobFactory
{
    /// <summary>
    /// Crée et exécute des travaux de sauvegarde en parallèle.
    /// </summary>
    /// <param name="jobs">Liste des travaux de sauvegarde à exécuter.</param>
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