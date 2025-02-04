using EasySaveConsole.EasySaveNamespace;
using EasySaveConsole.EasySaveNamespace.Backup;

public class Program
{

    private BackupManager backupManager = new BackupManager();

    public void Start()
    {
        while (true)
        {
            Console.WriteLine("\n--- EasySave Menu ---");
            Console.WriteLine("1. Ajouter une sauvegarde");
            Console.WriteLine("2. Supprimer une sauvegarde");
            Console.WriteLine("3. Lister les sauvegardes");
            Console.WriteLine("4. Exécuter une sauvegarde");
            Console.WriteLine("5. Restaurer une sauvegarde");
            Console.WriteLine("6. Voir les logs");
            Console.WriteLine("7. Quitter");

            Console.Write("Choix : ");
            string choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1":
                    AjouterSauvegarde();
                    break;
                case "2":
                    SupprimerSauvegarde();
                    break;
                case "3":
                    ListerSauvegardes();
                    break;
                case "4":
                    ExecuterSauvegarde();
                    break;
                case "5":
                    RestaurerSauvegarde();
                    break;
                case "6":
                    break;
                case "7":
                    return;
            }
        }
    }

    private void AjouterSauvegarde()
    {
        if (backupManager.GetBackupJobs().Count >= 5)
        {
            Console.WriteLine("Impossible d'ajouter plus de 5 travaux de sauvegarde.");
            return;
        }
        Console.Write("Nom de la sauvegarde : ");
        string name = Console.ReadLine()!;

        Console.Write("Répertoire source : ");
        string source = Console.ReadLine()!;

        Console.Write("Répertoire cible : ");
        string target = Console.ReadLine()!;

        Console.Write("Type de sauvegarde (1: complète, 2: différentielle) : ");
        int type = int.Parse(Console.ReadLine()!);

        IBackupTypeStrategy strategy;
        if (type == 1)
        {
            strategy = new CompleteBackupStrategy();
        }
        else
        {
            strategy = new DifferentialBackupStrategy();
        }

        BackupJob job = new BackupJob(name, source, target, type == 1, strategy);
        backupManager.AddBackup(job);
        Console.WriteLine("Sauvegarde ajoutée !");
    }

    private void SupprimerSauvegarde()
    {
        ListerSauvegardes();
        Console.Write("Entrez l'index de la sauvegarde à supprimer : ");
        int index = int.Parse(Console.ReadLine()!);

        if (index >= 0 && index < backupManager.GetBackupJobs().Count)
        {
            backupManager.GetBackupJobs().RemoveAt(index);
            Console.WriteLine("Sauvegarde supprimée !");
        }
        else
        {
            Console.WriteLine("Index invalide.");
        }
    }

    private void ListerSauvegardes()
    {
        Console.WriteLine("\nListe des sauvegardes :");
        var jobs = backupManager.GetBackupJobs();
        if (jobs.Count == 0)
        {
            Console.WriteLine("Aucune sauvegarde trouvée.");
        }
        else
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                Console.WriteLine($"ID: {job.Id}");
                Console.WriteLine($"Nom : {job.Name}");
                Console.WriteLine($"Source : {job.Source}");
                Console.WriteLine($"Cible : {job.Target}");
                Console.WriteLine($"Type : {(job.IsFullBackup ? "Complète" : "Différentielle")}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }

    private void ExecuterSauvegarde()
    {
        ListerSauvegardes();
        Console.Write("Entrez l'index de la sauvegarde à exécuter : ");
        int index = int.Parse(Console.ReadLine()!);

        var jobs = backupManager.GetBackupJobs();
        if (index >= 0 && index < jobs.Count)
        {
            Console.WriteLine($"Lancement de la sauvegarde {jobs[index].Name}...");
            backupManager.ExecuteJob(jobs[index].Id);
            Console.WriteLine("Sauvegarde terminée !");
        }
        else
        {
            Console.WriteLine("Index invalide.");
        }
    }

    private void RestaurerSauvegarde()
    {
        ListerSauvegardes();
        Console.Write("Entrez l'index de la sauvegarde à restaurer : ");
        int index = int.Parse(Console.ReadLine()!);

        var jobs = backupManager.GetBackupJobs();
        if (index >= 0 && index < jobs.Count)
        {
            Console.WriteLine($"Restauration de la sauvegarde {jobs[index].Name}...");
            backupManager.RestoreJob(jobs[index].Id);
            Console.WriteLine("Restauration terminée !");
        }
        else
        {
            Console.WriteLine("Index invalide.");
        }
    }

    static void Main()
    {
        Program Interface = new Program();
        Interface.Start();
    }
}


