namespace EasySaveLogger.EasySaveNamespace.Logger;

using System.Text.Json;
using System.Xml.Serialization;

/// <summary>
/// Handles the logging of backup operations and supports dynamic switching between JSON and XML formats.
/// </summary>
public class Logger
{
    private readonly string _logDirectory;
    private ILogFormatter _logFormatter;

    /// <summary>
    /// Initializes the logger with the specified log formatter.
    /// Creates the log directory if it does not exist.
    /// </summary>
    /// <param name="logFormatter">The formatter to use for writing logs (JSON or XML).</param>
    public Logger(ILogFormatter logFormatter)
    {
        _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        Directory.CreateDirectory(_logDirectory);
        _logFormatter = logFormatter;
    }

    /// <summary>
    /// Updates the log formatter and converts existing logs to the new format.
    /// </summary>
    /// <param name="newFormatter">The new formatter to use (JSON or XML).</param>
    public void SetLogFormatter(ILogFormatter newFormatter)
    {
        ConvertExistingLogs(newFormatter);
        _logFormatter = newFormatter;
    }

    /// <summary>
    /// Adds a new log entry for a backup operation and writes it using the current log formatter.
    /// </summary>
    /// <param name="backupName">The name of the backup job.</param>
    /// <param name="sourcePath">The source directory path of the backup.</param>
    /// <param name="destinationPath">The target directory path of the backup.</param>
    /// <param name="transferTimeMs">The time taken to complete the backup, in milliseconds.</param>
    public void Log(string backupName, string sourcePath, string destinationPath, long transferTimeMs)
    {
        List<LogEntry> logs = LoadExistingLogs();

        string[] files = Directory.GetFiles(sourcePath);
        long fileSize = 0;
        foreach (string file in files)
        {
            fileSize += new FileInfo(file).Length;
        }

        logs.Add(new LogEntry
        {
            Timestamp = DateTime.Now.ToString("o"),
            JobName = backupName,
            SourcePath = sourcePath,
            TargetPath = destinationPath,
            FileSize = fileSize
        });
        _logFormatter.WriteLog(_logDirectory, logs);
    }

    /// <summary>
    /// Loads existing logs from the current log file (either JSON or XML).
    /// </summary>
    /// <returns>A list of log entries from the existing log file.</returns>
    private List<LogEntry> LoadExistingLogs()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string jsonPath = Path.Combine(_logDirectory, $"{date}.json");
        string xmlPath = Path.Combine(_logDirectory, $"{date}.xml");

        if (File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<LogEntry>>(jsonContent) ?? new List<LogEntry>();
        }
        else if (File.Exists(xmlPath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));
            using FileStream fileStream = new FileStream(xmlPath, FileMode.Open);
            return (List<LogEntry>)serializer.Deserialize(fileStream)!;
        }

        return new List<LogEntry>();
    }

    /// <summary>
    /// Converts existing logs from one format to another and deletes the old log file.
    /// </summary>
    /// <param name="newFormatter">The new formatter to use (JSON or XML).</param>
    private void ConvertExistingLogs(ILogFormatter newFormatter)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string jsonPath = Path.Combine(_logDirectory, $"{date}.json");
        string xmlPath = Path.Combine(_logDirectory, $"{date}.xml");

        List<LogEntry> existingLogs = LoadExistingLogs();
        if (existingLogs.Count > 0)
        {
            newFormatter.WriteLog(_logDirectory, existingLogs);
Console.WriteLine("Logs converted to new format.");
            if (newFormatter is XmlLogFormatter && File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
            }
            else if (newFormatter is JsonLogFormatter && File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }
        }
    }
}
