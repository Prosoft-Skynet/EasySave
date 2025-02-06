﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using EasySaveConsole.EasySaveNamespace;
using EasySaveConsole.EasySaveNamespace.Backup;
using EasySaveConsole.EasySaveNamespace.Language;
using EasySaveConsole.EasySaveNamespace.State;
using EasySaveLogger;

public class Program
{
    private EasySave easySave;
    private StateManager stateManager;

    private Logger logger = new Logger();
    private Language currentLanguage = new EnLanguage();
    private BackupManager backupManager = new BackupManager();

    public void Start()
    {
        easySave = EasySave.GetInstance();
        stateManager = new StateManager(); 
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
                    Console.WriteLine(easySave.GetText("menu.state.Contents"));
                    string stateJson = File.ReadAllText(stateManager.GetStateFilePath());
                    Console.WriteLine(stateJson);
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

    private void AjouterSauvegarde()
    {
        Console.Write(easySave.GetText("backup.name"));
        string name = Console.ReadLine()!;
        
        if (backupManager.GetBackupJobs().Exists(job => job.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine(easySave.GetText("name already in use"));
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

        Console.Write(easySave.GetText("backup.type"));
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
        Console.WriteLine(easySave.GetText("backup.add"));
    }

    private void SupprimerSauvegarde()
    {
        ListerSauvegardes();
        Console.Write(easySave.GetText("delete.index"));
        string name = Console.ReadLine()!;

        var job = backupManager.GetBackupJobs().FirstOrDefault(j => j.Name == name);

        if (job != null)
        {
            backupManager.GetBackupJobs().Remove(job);
            Console.WriteLine(easySave.GetText("delete.delete"));
        }
        else
        {
            Console.WriteLine(easySave.GetText("index.invalid"));
        }
    }

    private void ListerSauvegardes()
    {
        Console.WriteLine(easySave.GetText("list.list"));
        var jobs = backupManager.GetBackupJobs();
        if (jobs.Count == 0)
        {
            Console.WriteLine(easySave.GetText("list.none"));
        }
        else
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                Console.WriteLine($"{easySave.GetText("list.none")}{job.Name}");
                Console.WriteLine($"Source : {job.Source}");
                Console.WriteLine($"{easySave.GetText("list.target")}{job.Target}");
                Console.WriteLine($"Type : {(job.IsFullBackup ? easySave.GetText("list.complete") : easySave.GetText("list.differential"))}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }

    private void ExecuterSauvegarde()
    {
        var debutTime = DateTime.Now;

        ListerSauvegardes();
        Console.Write(easySave.GetText("exec.index"));
        string name = Console.ReadLine()!;


        var job = backupManager.GetBackupJobs().FirstOrDefault(j => j.Name == name);

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
            Console.WriteLine(easySave.GetText("index.invalid"));
        }

        
    }

    private void RestaurerSauvegarde()
    {
        ListerSauvegardes();
        Console.Write(easySave.GetText("restore.index"));
        int index = int.Parse(Console.ReadLine()!);

        var jobs = backupManager.GetBackupJobs();
        if (index >= 0 && index < jobs.Count)
        {
            Console.WriteLine($"{easySave.GetText("restore.restore")}{jobs[index].Name}...");
            backupManager.RestoreJob(jobs[index].Id);
            Console.WriteLine(easySave.GetText("restore.finish"));
        }
        else
        {
            Console.WriteLine(easySave.GetText("index.invalid"));
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

    static void Main()
    {
        Program Interface = new Program();
        Interface.Start();
    }
}
