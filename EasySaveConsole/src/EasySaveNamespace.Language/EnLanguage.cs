namespace EasySaveConsole.EasySaveNamespace.Language;

/// <summary>
/// Implémentation de la classe Language pour la langue anglaise.
/// </summary>
public class EnLanguage : Language
{
    /// <summary>
    /// Retourne un dictionnaire contenant les traductions en anglais.
    /// </summary>
    /// <returns>Dictionnaire des clés de traduction avec leur texte en anglais.</returns>
    public override Dictionary<string, string> GetTranslations()
    {
        return new Dictionary<string, string>
        {
            { "menu.add", "1. Add a backup" },
            { "menu.delete", "2. Delete a backup" },
            { "menu.list", "3. List backups" },
            { "menu.execute", "4. Execute a backup" },
            { "menu.restore", "5. Restore a backup" },
            { "menu.logs", "6. View logs" },
            { "menu.state", "7. View state" },
            { "menu.state.Contents", "Contents of the state" },
            { "menu.language", "8. FR" },
            { "menu.quit", "9. Quit" },
            { "menu.choice", "Your choice: " },
            { "backup.error_5", "Impossible to add more than 5 backup jobs." },
            { "backup.name", "Backup name : " },
            { "backup.source", "Source directory : " },
            { "backup.destination", "Target directory : " },
            { "backup.type", "Backup type (1: full, 2: differential) : " },
            { "backup.add", "Backup added !" },
            { "delete.index", "Enter the index of the backup to be deleted : " },
            { "delete.delete", "Backup deleted !" },
            { "index.invalid", "Invalid index." },
            { "list.list", "\nList of backups :" },
            { "list.none", "No backups found." },
            { "list.name", "Name : " },
            { "list.target", "Target : " },
            { "list.complete", "Complete" },
            { "list.differential", "Differential" },
            { "exec.index", "Enter the index of the backup you want to run : " },
            { "exec.launch", "Launching the backup " },
            { "exec.finish", "Backup complete !" },
            { "restore.index", "Enter the index of the backup you wish to restore : " },
            { "restore.restore", "Restoring the backup " },
            { "restore.finish", "Restoration complete !" }
        };
    }
}

