namespace EasySaveLogger.Logger;

using System.Text.Json;
using System.Xml.Serialization;

/// <summary>
/// Handles the logging of backup operations and supports dynamic switching between JSON and XML formats.
/// </summary>
public class Logger
{
    private readonly string _jsonLogDirectory;
    private readonly string _xmlLogDirectory;
    private ILogFormatter _logFormatter;

    /// <summary>
    /// Initializes the logger with the specified log formatter.
    /// Creates the log directories if they do not exist.
    /// </summary>
    /// <param name="logFormatter">The formatter to use for writing logs (JSON or XML).</param>
    public Logger(ILogFormatter logFormatter)
    {
        _jsonLogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "JSON");
        _xmlLogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "XML");
        Directory.CreateDirectory(_jsonLogDirectory);
        Directory.CreateDirectory(_xmlLogDirectory);
        _logFormatter = logFormatter;
    }

    /// <summary>
    /// Updates the log formatter.
    /// </summary>
    /// <param name="newFormatter">The new formatter to use (JSON or XML).</param>
    public void SetLogFormatter(ILogFormatter newFormatter)
    {
        _logFormatter = newFormatter;
    }

    /// <summary>
    /// Give the log formatter.
    /// </summary>
    public ILogFormatter GetLogFormatter()
    {
        return _logFormatter;
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
            FileSize = fileSize,
            TransferTime = transferTimeMs
        });

        // Write logs in both JSON and XML formats
        WriteLogInBothFormats(logs);
    }

    /// <summary>
    /// Loads existing logs from the current log file (either JSON or XML).
    /// </summary>
    /// <returns>A list of log entries from the existing log file.</returns>
    private List<LogEntry> LoadExistingLogs()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string jsonPath = Path.Combine(_jsonLogDirectory, $"{date}.json");
        string xmlPath = Path.Combine(_xmlLogDirectory, $"{date}.xml");

        if (_logFormatter is JsonLogFormatter && File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<List<LogEntry>>(jsonContent) ?? new List<LogEntry>();
        }
        else if (_logFormatter is XmlLogFormatter && File.Exists(xmlPath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));
            using FileStream fileStream = new FileStream(xmlPath, FileMode.Open);
            return (List<LogEntry>)serializer.Deserialize(fileStream)!;
        }

        return new List<LogEntry>();
    }

    /// <summary>
    /// Writes logs in both JSON and XML formats.
    /// </summary>
    /// <param name="logs">The list of log entries to write.</param>
    private void WriteLogInBothFormats(List<LogEntry> logs)
    {
        ILogFormatter jsonFormatter = new JsonLogFormatter();
        ILogFormatter xmlFormatter = new XmlLogFormatter();

        jsonFormatter.WriteLog(_jsonLogDirectory, logs);
        xmlFormatter.WriteLog(_xmlLogDirectory, logs);
    }
}