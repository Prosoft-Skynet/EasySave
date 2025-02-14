namespace EasySaveCore.Language;

/// <summary>
/// Implementation of the Language class for the English language.
/// </summary>
public class EnLanguage : Language
{
    /// <summary>
    /// Returns a dictionary containing English translations.
    /// </summary>
    /// <returns>Dictionary of translation keys with their English text.</returns>
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
            { "menu.logs_format_JSON", "8. JSON" },
            { "menu.logs_format_XML", "8. XML" },
            { "menu.language", "9. FR" },
            { "menu.quit", "10. Quit" },
            { "menu.choice", "Your choice: " },
            { "backup.name", "Backup name : " },
            { "backup.name_use", "Name already in use" },
            { "backup.source", "Source directory : " },
            { "backup.destination", "Target directory : " },
            { "backup.type", "Backup type (1: full, 2: differential) : " },
            { "backup.add", "Backup added !" },
            { "delete.name", "Enter the name of the backup to be deleted : " },
            { "delete.delete", "Backup deleted !" },
            { "name.invalid", "Invalid name." },
            { "list.list", "\nList of backups :" },
            { "list.none", "No backups found." },
            { "list.name", "Name : " },
            { "list.target", "Target : " },
            { "list.complete", "Complete" },
            { "list.differential", "Differential" },
            { "exec.name", "Enter the name of the backup you want to run : " },
            { "exec.launch", "Launching the backup " },
            { "exec.finish", "Backup complete !" },
            { "restore.name", "Enter the name of the backup you wish to restore : " },
            { "restore.restore", "Restoring the backup " },
            { "restore.finish", "Restoration complete !" },
            { "logs.chose", "chose the log to execute" },
            { "logs.JSON_format", "Displaying logs in JSON" },
            { "logs.XML_format", "Displaying logs in XML" },
            { "logs.none", "there is no log for the moment" }
        };
    }
}

