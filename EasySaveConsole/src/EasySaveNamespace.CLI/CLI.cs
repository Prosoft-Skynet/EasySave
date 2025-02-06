namespace EasySaveConsole.EasySaveNamespace.CLI;

using EasySaveConsole.EasySaveNamespace.State;

/// <summary>
/// Classe responsable de l'interface en ligne de commande.
/// </summary>
public class CLI
{
    private BackupManager backupManager;
    private StateManager stateManager;

    private EasySave easySave = EasySave.GetInstance();

    /// <summary>
    /// Initialise une nouvelle instance de la classe CLI.
    /// </summary>
    /// <param name="backupManager">Instance de BackupManager pour gérer les sauvegardes.</param>
    /// <param name="stateManager">Instance de StateManager pour gérer l'état des sauvegardes.</param>
    public CLI(BackupManager backupManager, StateManager stateManager)
    {
        this.backupManager = backupManager;
        this.stateManager = stateManager;
    }

    /// <summary>
    /// Analyse et exécute une commande.
    /// </summary>
    /// <param name="command">Commande à analyser et exécuter.</param>
    public void ParseCommand(string command)
    {
        var parts = command.Split(' ', 2);
        var action = parts[0].Trim().ToLower();
        var parameters = parts.Length > 1 ? parts[1].Trim() : string.Empty;

        switch (action)
        {
            case "execute":
                ExecuteCommand(parameters);
                break;
            case "delete":
                DeleteCommand(parameters);
                break;
            case "restore":
                RestoreCommand(parameters);
                break;
            default:
                Console.WriteLine($"Invalid command: {action}");
                break;
        }
    }

    private void ExecuteCommand(string parameters)
    {
        var commands = parameters.Split(';');
        var positions = new List<int>();

        foreach (var cmd in commands)
        {
            if (cmd.Contains('-'))
            {
                var range = cmd.Split('-');
                if (range.Length == 2 && int.TryParse(range[0], out var start) && int.TryParse(range[1], out var end))
                {
                    for (int i = start; i <= end; i++)
                    {
                        positions.Add(i);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid range command: {cmd}");
                }
            }
            else if (int.TryParse(cmd, out var position))
            {
                positions.Add(position);
            }
            else
            {
                Console.WriteLine($"Invalid command: {cmd}");
            }
        }

        if (positions.Count > 0)
        {
            ExecuteJobsByPosition(positions);
        }
    }

    private void DeleteCommand(string parameters)
    {
        var positions = ParsePositions(parameters);
        if (positions.Count > 0)
        {
            DeleteJobsByPosition(positions);
        }
    }

    private void RestoreCommand(string parameters)
    {
        var positions = ParsePositions(parameters);
        if (positions.Count > 0)
        {
            RestoreJobsByPosition(positions);
        }
    }

    private List<int> ParsePositions(string parameters)
    {
        var commands = parameters.Split(';');
        var positions = new List<int>();

        foreach (var cmd in commands)
        {
            if (cmd.Contains('-'))
            {
                var range = cmd.Split('-');
                if (range.Length == 2 && int.TryParse(range[0], out var start) && int.TryParse(range[1], out var end))
                {
                    for (int i = start; i <= end; i++)
                    {
                        positions.Add(i);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid range command: {cmd}");
                }
            }
            else if (int.TryParse(cmd, out var position))
            {
                positions.Add(position);
            }
            else
            {
                Console.WriteLine($"Invalid command: {cmd}");
            }
        }

        return positions;
    }

    private void ExecuteJobsByPosition(List<int> positions)
    {
        var jobs = backupManager.GetBackupJobs();

        foreach (var position in positions)
        {
            if (position > 0 && position <= jobs.Count)
            {
                var job = jobs[position - 1];
                Console.WriteLine($"{easySave.GetText("exec.launch")}{job.Name}...");
                backupManager.ExecuteJob(job.Id);
                Console.WriteLine(easySave.GetText("exec.finish"));
            }
            else
            {
                Console.WriteLine($"Invalid position: {position}");
            }
        }
    }

    private void DeleteJobsByPosition(List<int> positions)
    {
        var jobs = backupManager.GetBackupJobs();

        foreach (var position in positions.OrderByDescending(p => p))
        {
            if (position > 0 && position <= jobs.Count)
            {
                var job = jobs[position - 1];
                backupManager.GetBackupJobs().Remove(job);
                Console.WriteLine(easySave.GetText("delete.delete"));
            }
            else
            {
                Console.WriteLine($"Invalid position: {position}");
            }
        }
    }

    private void RestoreJobsByPosition(List<int> positions)
    {
        var jobs = backupManager.GetBackupJobs();

        foreach (var position in positions)
        {
            if (position > 0 && position <= jobs.Count)
            {
                var job = jobs[position - 1];
                Console.WriteLine($"{easySave.GetText("restore.restore")}{job.Name}...");
                backupManager.RestoreJob(job.Id);
                Console.WriteLine(easySave.GetText("restore.finish"));
            }
            else
            {
                Console.WriteLine($"Invalid position: {position}");
            }
        }
    }

    /// <summary>
    /// Affiche l'état actuel des sauvegardes.
    /// </summary>
    public void DisplayState()
    {
        var stateJson = File.ReadAllText(stateManager.GetStateFilePath());
        Console.WriteLine(stateJson);
    }

    /// <summary>
    /// Affiche un message d'erreur.
    /// </summary>
    /// <param name="message">Message d'erreur à afficher.</param>
    public void DisplayError(string message)
    {
        Console.WriteLine($"Error: {message}");
    }

}