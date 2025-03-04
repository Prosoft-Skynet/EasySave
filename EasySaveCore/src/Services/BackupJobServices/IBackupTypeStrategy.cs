namespace EasySaveCore.src.Services.BackupJobServices;

using System.Collections.ObjectModel;

/// <summary>
/// Defines the backup strategy interface.
/// </summary>
public interface IBackupTypeStrategy
{
    /// <summary>
    /// Executes the backup strategy.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="target">The target directory.</param>
    void ExecuteBackupStrategy(string source, string target, ObservableCollection<string> filesExceptions, Action checkForPauseAndStop);
}
