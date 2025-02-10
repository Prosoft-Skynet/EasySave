namespace EasySaveLogger.EasySaveNamespace.Logger;

/// <summary>
/// Interface for defining custom log formatters (e.g., JSON, XML).
/// Implementations of this interface are responsible for writing log entries 
/// in a specific format to a log file.
/// </summary>
public interface ILogFormatter
{
    /// <summary>
    /// Writes a list of log entries to a log file in the specified format.
    /// </summary>
    /// <param name="logDirectory">The directory where the log file should be created or appended.</param>
    /// <param name="logs">The list of log entries to be written.</param>
    void WriteLog(string logDirectory, List<LogEntry> logs);
}