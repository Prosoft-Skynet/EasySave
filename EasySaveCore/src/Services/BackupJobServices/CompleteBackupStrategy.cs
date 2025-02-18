using System.Collections.ObjectModel;

namespace EasySaveCore.src.Services.BackupJobServices;

using System;
using System.IO;
using System.Linq;

/// <summary>
/// Implements the complete backup strategy.
/// </summary>
public class CompleteBackupStrategy : IBackupTypeStrategy
{
    /// <summary>
    /// Executes a complete backup.
    /// </summary>
    /// <param name="source">The source directory.</param>
    /// <param name="target">The target directory.</param>
    public void ExecuteBackupStrategy(string source, string target, ObservableCollection<string> filesExceptions)
    {
        if (!Directory.Exists(source))
        {
            throw new DirectoryNotFoundException($"Le répertoire source {source} n'existe pas.");
        }

        if (!Directory.Exists(target))
        {
            Directory.CreateDirectory(target);
        }

        var sourceFiles = Directory.GetFiles(source).Select(Path.GetFileName).ToHashSet();

        foreach (var file in Directory.GetFiles(target))
        {
            var fileName = Path.GetFileName(file);
            if (!sourceFiles.Contains(fileName))
            {
                File.Delete(file);
            }
        }

        var backupService = new BackupService();
        backupService.TransferFiles(source, target, filesExceptions);
    }
}
