namespace EasySaveCore.src.Models;

using System;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;
using EasySaveCore.src.Services.BackupJobServices;

/// <summary>
/// Represents a backup job with details such as name, source, target, and backup type.
/// </summary>
public class BackupJobModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public bool IsFullBackup { get; set; }

    public BackupJobModel()
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        Source = string.Empty;
        Target = string.Empty;
        IsFullBackup = false;
    }


    /// <summary>
    /// Initializes a new instance of the BackupJob class.
    /// </summary>
    /// <param name="name">The name of the backup job.</param>
    /// <param name="source">The source directory.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="isFullBackup">Indicates whether the backup is a full backup.</param>
    /// <param name="strategy">The backup strategy to use.</param>
    public BackupJobModel(string name, string source, string target, bool isFullBackup)
    {
        Id = Guid.NewGuid();
        Name = name;
        Source = source;
        Target = target;
        IsFullBackup = isFullBackup;
    }
}
