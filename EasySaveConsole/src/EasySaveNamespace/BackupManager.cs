using EasySaveConsole.EasySaveNamespace.Backup;
using EasySaveConsole.EasySaveNamespace.State;

namespace EasySaveConsole.EasySaveNamespace;

public class BackupManager
{
    private List<BackupJob> backupJobs = new List<BackupJob>();
    private StateManager stateManager = new StateManager();

    /// <summary>
    /// Ajoute un nouveau travail de sauvegarde.
    /// </summary>
    /// <param name="job">Travail de sauvegarde à ajouter.</param>
    public void AddBackup(BackupJob job)
    {
        backupJobs.Add(job);
    }

    /// <summary>
    /// Exécute un travail de sauvegarde spécifié par son identifiant.
    /// </summary>
    /// <param name="jobId">Identifiant du travail de sauvegarde.</param>
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

    /// <summary>
    /// Exécute séquentiellement une liste de travaux de sauvegarde.
    /// </summary>
    /// <param name="jobIds">Liste des identifiants des travaux de sauvegarde.</param>
    public void ExecuteJobsSequentially(List<Guid> jobIds)
    {
        foreach (var jobId in jobIds)
        {
            ExecuteJob(jobId);
        }
    }

    /// <summary>
    /// Restaure un travail de sauvegarde spécifié par son identifiant.
    /// </summary>
    /// <param name="jobId">Identifiant du travail de sauvegarde.</param>
    public void RestoreJob(Guid jobId)
    {
        var job = backupJobs.FirstOrDefault(j => j.Id == jobId);
        if (job != null)
        {
            job.Restore();
        }
    }

    /// <summary>
    /// Récupère la liste des travaux de sauvegarde.
    /// </summary>
    /// <returns>Liste des travaux de sauvegarde.</returns>
    public List<BackupJob> GetBackupJobs()
    {
        return backupJobs;
    }
}