namespace EasySaveConsole.EasySaveNamespace.Backup;

public class DifferentialBackupStrategy : IBackupTypeStrategy
{
    public void ExecuteBackupStrategy(string source, string target)
    {
        var sourceDirectory = new DirectoryInfo(source);

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
