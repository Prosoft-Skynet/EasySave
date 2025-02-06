using System.Diagnostics;
using System.Runtime.InteropServices;
using EasySaveConsole.EasySaveNamespace;
using EasySaveConsole.EasySaveNamespace.Backup;
using EasySaveConsole.EasySaveNamespace.CLI;
using EasySaveConsole.EasySaveNamespace.Language;
using EasySaveConsole.EasySaveNamespace.State;
using EasySaveLogger;

/// <summary>
/// Represents the main program of EasySave.
/// </summary>
public class Program
{

    private EasySave easySave = EasySave.GetInstance();
    private static StateManager stateManager = new StateManager();

    private Logger logger = new Logger();
    private Language currentLanguage = new EnLanguage();
    private static BackupManager backupManager = new BackupManager();
    private CLI cli = new CLI(backupManager, stateManager);

    /// <summary>
    /// Starts the EasySave console interface.
    /// </summary>
    public void Start()
    {
        while (true)
        {
            Console.WriteLine("\n--- EasySave Menu ---");
            Console.WriteLine(easySave.GetText("menu.add"));
            Console.WriteLine(easySave.GetText("menu.delete"));
            Console.WriteLine(easySave.GetText("menu.list"));
            Console.WriteLine(easySave.GetText("menu.execute"));
            Console.WriteLine(easySave.GetText("menu.restore"));
            Console.WriteLine(easySave.GetText("menu.logs"));
            Console.WriteLine(easySave.GetText("menu.state"));
            Console.WriteLine(easySave.GetText("menu.language"));
            Console.WriteLine(easySave.GetText("menu.quit"));
            Console.Write(easySave.GetText("menu.choice"));
            string choice = Console.ReadLine()!;

            if (choice.StartsWith("execute") || choice.StartsWith("delete") || choice.StartsWith("restore"))
            {

                cli.ParseCommand(choice);
            }
            else
            {
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
                        ReadLogs();
                        break;
                    case "7":
                        cli.DisplayState();
                        break;
                    case "8":
                        if (currentLanguage is FrLanguage)
                        {
                            currentLanguage = new EnLanguage();
                        }
                        else
                        {
                            currentLanguage = new FrLanguage();
                        }
                        easySave.SetLanguage(currentLanguage);
                        break;
                    case "9":
                        return;
                }
            }
        }
    }

    /// <summary>
    /// Adds a new backup job to the list of backup jobs.
    /// </summary>
    private void AjouterSauvegarde()
    {
        Console.Write(easySave.GetText("backup.name"));
        string name = Console.ReadLine()!;

        if (backupManager.GetBackupJobs().Exists(job => job.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine(easySave.GetText("backup.name_use"));
            return;
        }

        if (backupManager.GetBackupJobs().Count >= 5)
        {
            Console.WriteLine(easySave.GetText("backup.error_5"));
            return;
        }

        Console.Write(easySave.GetText("backup.source"));
        string source = Console.ReadLine()!;

        Console.Write(easySave.GetText("backup.destination"));
        string target = Console.ReadLine()!;

        int type;
        do
        {
            Console.Write(easySave.GetText("backup.type"));
        } while (!int.TryParse(Console.ReadLine(), out type) || (type != 1 && type != 2));

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
        Console.WriteLine(easySave.GetText("backup.add"));
    }

    /// <summary>
    /// Deletes a backup job from the list of backup jobs.
    /// </summary>
    private void SupprimerSauvegarde()
    {
        if (backupManager.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(easySave.GetText("list.none"));
            return;
        }
        ListerSauvegardes();
        Console.Write(easySave.GetText("delete.name"));
        string name = Console.ReadLine()!;

        var job = backupManager.GetBackupJobs().FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (job != null)
        {
            backupManager.GetBackupJobs().Remove(job);
            Console.WriteLine(easySave.GetText("delete.delete"));
        }
        else
        {
            Console.WriteLine(easySave.GetText("name.invalid"));
        }
    }

    /// <summary>
    /// Lists all backup jobs.    
    /// </summary>
    private void ListerSauvegardes()
    {

        if (backupManager.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(easySave.GetText("list.none"));
            return;
        }
        else
        {
            Console.WriteLine(easySave.GetText("list.list"));
            var jobs = backupManager.GetBackupJobs();
            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];

                Console.WriteLine($"{easySave.GetText("list.name")}{job.Name}");
                Console.WriteLine($"Source : {job.Source}");
                Console.WriteLine($"{easySave.GetText("list.target")}{job.Target}");
                Console.WriteLine($"Type : {(job.IsFullBackup ? easySave.GetText("list.complete") : easySave.GetText("list.differential"))}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }

    /// <summary>
    /// Executes a backup job.
    /// </summary>
    private void ExecuterSauvegarde()
    {
        var debutTime = DateTime.Now;

        if (backupManager.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(easySave.GetText("list.none"));
            return;
        }
        ListerSauvegardes();
        Console.Write(easySave.GetText("exec.name"));
        string name = Console.ReadLine()!;

        var jobs = backupManager.GetBackupJobs();
        var job = backupManager.GetBackupJobs().FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (job != null)
        {
            Console.WriteLine($"{easySave.GetText("exec.launch")}{job.Name}...");
            backupManager.ExecuteJob(job.Id);
            Console.WriteLine(easySave.GetText("exec.finish"));

            long savingTime = DateTime.Now.Millisecond - debutTime.Millisecond;

            this.logger.Log(job.Name, job.Source, job.Target, savingTime);
        }
        else
        {
            Console.WriteLine(easySave.GetText("name.invalid"));
        }

        
    }

    /// <summary>
    /// Restores a backup job.
    /// </summary>
    private void RestaurerSauvegarde()
    {
        if (backupManager.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(easySave.GetText("list.none"));
            return;
        }
        ListerSauvegardes();
        Console.Write(easySave.GetText("restore.name"));
        string name = Console.ReadLine()!;

        var jobs = backupManager.GetBackupJobs();
        var job = jobs.FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (job != null)
        {
            Console.WriteLine($"{easySave.GetText("restore.restore")}{job.Name}...");
            backupManager.RestoreJob(job.Id);
            Console.WriteLine(easySave.GetText("restore.finish"));
        }
        else
        {
            Console.WriteLine(easySave.GetText("name.invalid"));
        }
    }

    private void ReadLogs()
    {
        var logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        var files = Directory.GetFiles(logsPath)
                     .Select(Path.GetFileName)
                     .ToList();
        var filesName = Directory.GetFiles(logsPath)
                     .Select(Path.GetFileNameWithoutExtension)
                     .ToList();
        if (files.Count > 0)
        {
            var fileToOpen = "";
            var chosenFile = "";
            while (!filesName.Contains(chosenFile))
            {
                Console.WriteLine(easySave.GetText("logs.chose"));
                foreach (var file in filesName)
                {
                    Console.WriteLine(file);
                }
                chosenFile = Console.ReadLine();

                for (int i = 0; i < filesName.Count; i++)
                {
                    if(chosenFile == filesName[i])
                    {
                        fileToOpen = files[i];
                    }
                }

            }


            var filePath = Path.Combine(logsPath, fileToOpen);




            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("notepad.exe", filePath); // Windows : Ouvre avec le Bloc-notes
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("gedit", filePath); // Linux : Ouvre avec Gedit (Bloc-notes par défaut sur GNOME)
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", "-a TextEdit " + filePath); // macOS : Ouvre avec TextEdit
            }
            else
            {
                Console.WriteLine("Système d'exploitation non supporté.");
            }


        }
        else
        {
            Console.WriteLine(easySave.GetText("logs.none"));
        }


    }

    /// <summary>
    /// Main entry point of the program.
    /// </summary>
    static void Main()
    {
        Program Interface = new Program();
        Interface.Start();
    }
}
