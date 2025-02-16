using EasySaveCore.State;
using EasySaveCore.Controller;

/// <summary>
/// Class responsible for the command line interface.
/// </summary>
public class CLI
{
    private BackupManager backupManager;
    private StateManager stateManager;
    private EasySave easySave = EasySave.GetInstance();

    /// <summary>
    /// Initializes a new instance of the CLI class.
    /// </summary>
    /// <param name="backupManager">Instance of BackupManager to manage backups.</param>
    /// <param name="stateManager">StateManager instance for managing the state of backups.</param>
    public CLI(BackupManager backupManager, StateManager stateManager)
    {
        this.backupManager = backupManager;
        this.stateManager = stateManager;
    }

    /// <summary>
    /// Analyses and executes a command.
    /// </summary>
    /// <param name="command">Order to be analysed and executed.</param>
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

    /// <summary>
    /// Parses a command string containing job positions or ranges and executes the corresponding jobs.
    /// </summary>
    /// <param name="parameters">
    /// A string containing job positions separated by ';'. 
    /// It can include individual numbers ("1;3;5") and ranges ("2-4").
    /// </param>
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

    /// <summary>
    /// Parses a command string to extract job positions and deletes the corresponding jobs.
    /// </summary>
    private void DeleteCommand(string parameters)
    {
        var positions = ParsePositions(parameters);
        if (positions.Count > 0)
        {
            DeleteJobsByPosition(positions);
        }
    }

    /// <summary>
    /// Parses a command string to extract job positions and restores the corresponding jobs.
    /// </summary>
    private void RestoreCommand(string parameters)
    {
        var positions = ParsePositions(parameters);
        if (positions.Count > 0)
        {
            RestoreJobsByPosition(positions);
        }
    }

    /// <summary>
    /// Parses a command string to extract job positions from individual numbers or ranges.
    /// </summary>
    /// <param name="parameters">
    /// A string containing job positions separated by ';'.
    /// It can include individual numbers (e.g., "1;3;5") and ranges (e.g., "2-4").
    /// </param>
    /// <returns>
    /// A list of integers representing the parsed job positions.
    /// </returns>
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

    /// <summary>
    /// Executes backup jobs based on their positions in the job list.
    /// </summary>
    /// <param name="positions">
    /// A list of integers representing the positions of the backup jobs to execute.
    /// </param>
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

    /// <summary>
    /// Deletes backup jobs based on their positions in the job list.
    /// </summary>
    /// <param name="positions">
    /// A list of integers representing the positions of the backup jobs to delete.
    /// </param>
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

    /// <summary>
    /// Restores backup jobs based on their positions in the job list.
    /// </summary>
    /// <param name="positions">
    /// A list of integers representing the positions of the backup jobs to restore.
    /// </param>
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
    /// Displays the current status of backups.
    /// </summary>
    public void DisplayState()
    {
        var stateJson = File.ReadAllText(stateManager.GetStateFilePath());
        Console.WriteLine(stateJson);
    }

    /// <summary>
    /// Displays an error message.
    /// </summary>
    /// <param name="message">Error message to be displayed.</param>
    public void DisplayError(string message)
    {
        Console.WriteLine($"Error: {message}");
    }
}
