namespace EasySaveConsole.EasySaveNamespace.Backup;

public class BackupService
{
    /// <summary>
    /// Transfère les fichiers et répertoires du répertoire source vers le répertoire cible.
    /// </summary>
    /// <param name="source">Répertoire source.</param>
    /// <param name="target">Répertoire cible.</param>
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
    /// Crée les répertoires nécessaires.
    /// </summary>
    /// <param name="path">Chemin du répertoire à créer.</param>
    public void CreateDirectories(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}