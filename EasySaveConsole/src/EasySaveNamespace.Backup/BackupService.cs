namespace EasySaveConsole.EasySaveNamespace.Backup;

using System.IO;

public class BackupService
{
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

    public void CreateDirectories(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
