namespace EasySaveConsole.EasySaveNamespace.State;

/// <summary>
/// Represents the state of a backup job.
/// Contains information about the job such as name, status, progress, size, and remaining files.
/// </summary>
public class StateEntry
{
    /// <summary>
    /// Name of the backup job.
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    /// Timestamp indicating when the job state was recorded.
    /// </summary>
    public string Timestamp { get; set; }

    /// <summary>
    /// Current status of the job (e.g., "Active", "Completed", etc.).
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Total number of files to be backed up.
    /// </summary>
    public int FilesTotal { get; set; }

    /// <summary>
    /// Total size of the files to be backed up (in bytes).
    /// </summary>
    public int SizeTotal { get; set; }

    /// <summary>
    /// Job progress, between 0 (no progress) and 1 (fully completed).
    /// </summary>
    public float Progress { get; set; }

    /// <summary>
    /// Number of remaining files to be processed.
    /// </summary>
    public int RemainingFiles { get; set; }

    /// <summary>
    /// Size of the remaining files to be processed (in bytes).
    /// </summary>
    public int RemainingSize { get; set; }

    /// <summary>
    /// Current source of the backup job (source directory or file).
    /// </summary>
    public string CurrentSource { get; set; }

    /// <summary>
    /// Current destination of the backup (target directory).
    /// </summary>
    public string CurrentTarget { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateEntry"/> class.
    /// The default status is "Inactive" and the timestamp is set to the current date and time.
    /// </summary>
    public StateEntry()
    {
        JobName = string.Empty;
        CurrentSource = string.Empty;
        CurrentTarget = string.Empty;
        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Status = "Non Actif";
    }
}