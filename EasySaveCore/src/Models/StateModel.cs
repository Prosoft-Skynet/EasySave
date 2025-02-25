namespace EasySaveCore.src.Models;

/// <summary>
/// Represents the state of a backup job.
/// Contains information about the job such as name, status, progress, size, and remaining files.
/// </summary>
public class StateModel
{
    public string JobName { get; set; }
    public string Timestamp { get; set; }
    public string Status { get; set; }
    public int FilesTotal { get; set; }
    public int SizeTotal { get; set; }
    public float Progress { get; set; }
    public int RemainingFiles { get; set; }
    public int RemainingSize { get; set; }
    public string CurrentSource { get; set; }
    public string CurrentTarget { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateModel"/> class.
    /// The default status is "Inactive" and the timestamp is set to the current date and time.
    /// </summary>
    public StateModel()
    {
        JobName = string.Empty;
        CurrentSource = string.Empty;
        CurrentTarget = string.Empty;
        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Status = "Non Actif";
    }

}
