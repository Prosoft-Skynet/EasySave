namespace EasySaveLogger.Logger;

using System.Text.Json.Serialization;

/// <summary>
/// Represents an entry in the log file, containing details about a backup operation.
/// </summary>
public class LogEntry
{

    [JsonPropertyName("timestamp")]
    public required string Timestamp { get; set; }

    [JsonPropertyName("backupName")]
    public required string JobName { get; set; }

    [JsonPropertyName("sourcePath")]
    public required string SourcePath { get; set; }

    [JsonPropertyName("destinationPath")]
    public required string TargetPath { get; set; }

    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }

    [JsonPropertyName("transferTimeMs")]
    public long TransferTime { get; set; }
    
    [JsonPropertyName("encryptionTimeMs")]
    public long EncryptionTime { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with default values.
    /// </summary>
    public LogEntry()
    {
        Timestamp = string.Empty;
        JobName = string.Empty;
        SourcePath = string.Empty;
        TargetPath = string.Empty;
        FileSize = 0;
        TransferTime = 0;
        EncryptionTime = 0;

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with the specified values.
    /// </summary>
    /// <param name="timestamp">The timestamp of the backup operation in ISO 8601 format.</param>
    /// <param name="jobName">The name of the backup job.</param>
    /// <param name="sourcePath">The source directory path of the backup operation.</param>
    /// <param name="targetPath">The target directory path of the backup operation.</param>
    /// <param name="fileSize">The total size of the files backed up, in bytes.</param>
    /// <param name="transferTime">The time taken to complete the backup, in milliseconds.</param>
    public LogEntry(string timestamp, string jobName, string sourcePath, string targetPath, long fileSize, long transferTime, long encryptionTime)
    {
        this.Timestamp = timestamp;
        this.JobName = jobName;
        this.SourcePath = sourcePath;
        this.TargetPath = targetPath;
        this.FileSize = fileSize;
        this.TransferTime = transferTime;
        this.EncryptionTime = encryptionTime;

    }
}
