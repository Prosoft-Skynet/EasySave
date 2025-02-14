namespace EasySaveLogger.Logger;

using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// A log formatter that writes log entries to an XML file.
/// Implements the <see cref="ILogFormatter"/> interface.
/// </summary>
public class XmlLogFormatter : ILogFormatter
{
    /// <summary>
    /// Writes a list of log entries to a log file in XML format.
    /// If the log file does not exist, it will be created. Otherwise, the log entries will be appended.
    /// </summary>
    /// <param name="logDirectory">The directory where the XML log file is created or updated.</param>
    /// <param name="logs">The list of log entries to be serialized and written.</param>
    public void WriteLog(string logDirectory, List<LogEntry> logs)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string logFilePath = Path.Combine(logDirectory, $"{date}.xml");

        XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>));

        using StreamWriter writer = new StreamWriter(logFilePath);
        serializer.Serialize(writer, logs);
    }
}
