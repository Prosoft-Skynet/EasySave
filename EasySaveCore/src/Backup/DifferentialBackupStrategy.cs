namespace EasySaveCore.Backup;

/// <summary>
/// Implements the differential backup strategy.
/// </summary>
public class DifferentialBackupStrategy : IBackupTypeStrategy
{
    /// <summary>
    /// Executes a differential backup.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="target">The target directory.</param>
    public void ExecuteBackupStrategy(string source, string target)
    {
        var sourceDirectory = new DirectoryInfo(source);

        if (!Directory.Exists(target))
        {
            Directory.CreateDirectory(target);
        }

        foreach (var file in sourceDirectory.GetFiles())
        {
            var targetFilePath = Path.Combine(target, file.Name);

            if (!File.Exists(targetFilePath) || File.GetLastWriteTime(targetFilePath) < file.LastWriteTime)
            {
                file.CopyTo(targetFilePath, true);
            }
        }

        foreach (var directory in sourceDirectory.GetDirectories())
        {
            var targetSubDirPath = Path.Combine(target, directory.Name);

            ExecuteBackupStrategy(directory.FullName, targetSubDirPath);
        }
    }
}
