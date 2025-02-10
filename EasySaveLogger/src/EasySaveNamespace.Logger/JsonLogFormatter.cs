namespace EasySaveLogger.EasySaveNamespace.Logger;

using System.Text.Json;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// A log formatter that writes log entries to a JSON file.
/// Implements the <see cref="ILogFormatter"/> interface.
/// </summary>
public class JsonLogFormatter : ILogFormatter
{
    /// <summary>
    /// Writes a list of log entries to a log file in JSON format.
    /// If the log file does not exist, it will be created. Otherwise, the log entries will be appended.
    /// </summary>
    /// <param name="logDirectory">The directory where the JSON log file is created or updated.</param>
    /// <param name="logs">The list of log entries to be serialized and written.</param>
    public void WriteLog(string logDirectory, List<LogEntry> logs)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string logFilePath = Path.Combine(logDirectory, $"{date}.json");

        string json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(logFilePath, json);
    }
}
