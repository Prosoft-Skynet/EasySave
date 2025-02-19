using System.Diagnostics;
using System.Runtime.InteropServices;
using EasySaveLogger.Logger;
using EasySaveCore.src.Services;
using EasySaveCore.src.Models;
using EasySaveCore.src.Services.BackupJobServices;

/// <summary>
/// Represents the main program of EasySave.
/// </summary>
public class Program
{
    private static StateService stateManager = new StateService();
    private static BackupService _backupService = new BackupService();
    private LanguageService _languageService = new LanguageService();
    private CLI cli = new CLI(_backupService, stateManager);
    private Logger logger;

    public Program()
    {
        logger = new Logger(new JsonLogFormatter());
        logger.SetLogFormatter(new JsonLogFormatter());
    }

    /// <summary>
    /// Starts the EasySave console interface.
    /// </summary>
    public void Start()
    {
        while (true)
        {
            Console.WriteLine("\n--- EasySave Menu ---");
            Console.WriteLine(_languageService.GetTranslation("menu.add"));
            Console.WriteLine(_languageService.GetTranslation("menu.delete"));
            Console.WriteLine(_languageService.GetTranslation("menu.list"));
            Console.WriteLine(_languageService.GetTranslation("menu.execute"));
            Console.WriteLine(_languageService.GetTranslation("menu.restore"));
            Console.WriteLine(_languageService.GetTranslation("menu.logs"));
            Console.WriteLine(_languageService.GetTranslation("menu.state"));
            if (logger.GetLogFormatter() is JsonLogFormatter)
                Console.WriteLine(_languageService.GetTranslation("menu.logs_format_XML"));
            else
                Console.WriteLine(_languageService.GetTranslation("menu.logs_format_JSON"));
            Console.WriteLine(_languageService.GetTranslation("menu.language"));
            Console.WriteLine(_languageService.GetTranslation("menu.quit"));
            Console.Write(_languageService.GetTranslation("menu.choice"));
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
                        ChangerFormatLog();
                        break;
                    case "9":
                        _languageService.ChangeLanguage();
                        break;
                    case "10":
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
        Console.Write(_languageService.GetTranslation("backup.name"));
        string name = Console.ReadLine()!;

        if (_backupService.GetBackupJobs().Exists(job => job.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine(_languageService.GetTranslation("backup.name_use"));
            return;
        }

        Console.Write(_languageService.GetTranslation("backup.source"));
        string source = Console.ReadLine()!;

        Console.Write(_languageService.GetTranslation("backup.destination"));
        string target = Console.ReadLine()!;

        int type;
        do
        {
            Console.Write(_languageService.GetTranslation("backup.type"));
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

        BackupJobModel job = new BackupJobModel(name, source, target, type == 1);
        _backupService.AddBackup(job);
        Console.WriteLine(_languageService.GetTranslation("backup.add"));
    }

    /// <summary>
    /// Deletes a backup job from the list of backup jobs.
    /// </summary>
    private void SupprimerSauvegarde()
    {
        if (_backupService.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(_languageService.GetTranslation("list.none"));
            return;
        }
        ListerSauvegardes();
        Console.Write(_languageService.GetTranslation("delete.name"));
        string name = Console.ReadLine()!;

        var job = _backupService.GetBackupJobs().FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (job != null)
        {
            _backupService.GetBackupJobs().Remove(job);
            Console.WriteLine(_languageService.GetTranslation("delete.delete"));
        }
        else
        {
            Console.WriteLine(_languageService.GetTranslation("name.invalid"));
        }
    }

    /// <summary>
    /// Lists all backup jobs.    
    /// </summary>
    private void ListerSauvegardes()
    {
        if (_backupService.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(_languageService.GetTranslation("list.none"));
            return;
        }
        else
        {
            Console.WriteLine(_languageService.GetTranslation("list.list"));
            var jobs = _backupService.GetBackupJobs();
            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];

                Console.WriteLine($"{_languageService.GetTranslation("list.name")}{job.Name}");
                Console.WriteLine($"Source : {job.Source}");
                Console.WriteLine($"{_languageService.GetTranslation("list.target")}{job.Target}");
                Console.WriteLine($"Type : {(job.IsFullBackup ? _languageService.GetTranslation("list.complete") : _languageService.GetTranslation("list.differential"))}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }

    /// <summary>
    /// Executes a backup job.
    /// </summary>
    private void ExecuterSauvegarde()
    {
        var debutTime = DateTime.Now.Millisecond;

        if (_backupService.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(_languageService.GetTranslation("list.none"));
            return;
        }
        ListerSauvegardes();
        Console.Write(_languageService.GetTranslation("exec.name"));
        string name = Console.ReadLine()!;

        var job = _backupService.GetBackupJobs().FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (job != null)
        {
            Console.WriteLine($"{_languageService.GetTranslation("exec.launch")}{job.Name}...");
            _backupService.ExecuteJob(job.Id);
            Console.WriteLine(_languageService.GetTranslation("exec.finish"));

            long savingTime = DateTime.Now.Millisecond - debutTime;

            logger.Log(job.Name, job.Source, job.Target, savingTime);
        }
        else
        {
            Console.WriteLine(_languageService.GetTranslation("name.invalid"));
        }
    }

    /// <summary>
    /// Restores a backup job.
    /// </summary>
    private void RestaurerSauvegarde()
    {
        if (_backupService.GetBackupJobs().Count == 0)
        {
            Console.WriteLine(_languageService.GetTranslation("list.none"));
            return;
        }
        ListerSauvegardes();
        Console.Write(_languageService.GetTranslation("restore.name"));
        string name = Console.ReadLine()!;

        var jobs = _backupService.GetBackupJobs();
        var job = jobs.FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (job != null)
        {
            Console.WriteLine($"{_languageService.GetTranslation("restore.restore")}{job.Name}...");
            _backupService.Restore(job);
            Console.WriteLine(_languageService.GetTranslation("restore.finish"));
        }
        else
        {
            Console.WriteLine(_languageService.GetTranslation("name.invalid"));
        }
    }

    /// <summary>
    /// Changes the format of the log files between JSON and XML.
    /// </summary>
    private void ChangerFormatLog()
    {
        ILogFormatter currentFormatter = logger.GetLogFormatter();
        ILogFormatter newFormatter;

        if (currentFormatter is JsonLogFormatter)
        {
            newFormatter = new XmlLogFormatter();
            Console.WriteLine(_languageService.GetTranslation("logs.XML_format"));
        }
        else
        {
            newFormatter = new JsonLogFormatter();
            Console.WriteLine(_languageService.GetTranslation("logs.JSON_format"));
        }

        logger.SetLogFormatter(newFormatter);
    }

    /// <summary>
    /// Reads the logs from the log files.
    /// </summary>
    private void ReadLogs()
    {
        string logsPath;
        if (logger.GetLogFormatter() is JsonLogFormatter)
        {
            logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "JSON");
        }
        else
        {
            logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "XML");
        }

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
                Console.WriteLine(_languageService.GetTranslation("logs.chose"));
                foreach (var file in filesName)
                {
                    Console.WriteLine(file);
                }
                chosenFile = Console.ReadLine()!;

                for (int i = 0; i < filesName.Count; i++)
                {
                    if (chosenFile == filesName[i])
                    {
                        fileToOpen = files[i];
                    }
                }
            }

            var filePath = Path.Combine(logsPath, fileToOpen!);

            Thread.Sleep(100);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("notepad.exe", filePath); // Windows : open it with Notebloc
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("gedit", filePath); // Linux : open it with Gedit (default GNOME notebloc)
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", "-a TextEdit " + filePath); // macOS : open it with TextEdit
            }
            else
            {
                Console.WriteLine("Système d'exploitation non supporté.");
            }
        }
        else
        {
            Console.WriteLine(_languageService.GetTranslation("logs.none"));
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
