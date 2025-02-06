namespace EasySaveConsole.EasySaveNamespace.Backup;

/// <summary>
/// Provides services for transferring files and creating directories.
/// </summary>
public class BackupService
{
    /// <summary>
    /// Transfers files and directories from the source directory to the target directory.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="target">The target directory.</param>
    public void TransferFiles(string source, string target)
    {
        var sourceDirectory = new DirectoryInfo(source);
        var targetDirectory = new DirectoryInfo(target);

        if (!targetDirectory.Exists)
        {
            CreateDirectories(target);
        }

        foreach (var file in sourceDirectory.GetFiles())
        {
            file.CopyTo(Path.Combine(target, file.Name), true);
        }

        foreach (var directory in sourceDirectory.GetDirectories())
        {
            TransferFiles(directory.FullName, Path.Combine(target, directory.Name));
        }
    }

    /// <summary>
    /// Creates the necessary directories.
    /// </summary>
    /// <param name="path">The path of the directory to create.</param>
    public void CreateDirectories(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}