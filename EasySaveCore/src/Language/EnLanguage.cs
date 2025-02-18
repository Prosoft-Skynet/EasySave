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
            { "menu.add", "Add a backup" },
            { "menu.delete", "Delete a backup" },
            { "menu.list", "List of backups :" },
            { "menu.execute", "Execute a backup" },
            { "menu.restore", "Restore a backup" },
            { "menu.logs", "View logs" },
            { "menu.state", "View state" },
            { "menu.create", "Create a new backup" },
            { "menu.name", "Backup name :" },
            { "menu.source", "Source folder :" },
            { "menu.target", "Target folder :" },
            { "menu.type", "Backup type :" },
            { "menu.complete", "Complete" },
            { "menu.differential", "Differential" },
            { "menu.params", "Settings" },
            { "menu.quit", "Quit" },
            { "menu.change_logs", "Change Logs format (XML/JSON)" },
            { "box.error", "Error" },
            { "box.fill", "Please fill all fields." },
            { "box.name", "A backup with this name already exists." },
            { "box.files", "Select a folder" },
            { "box.backup", "Backup" },
            { "box.create_success", "created successfully !" },
            { "box.delete", "Select a backup to delete." },
            { "box.delete_success", "deleted successfully !" },
            { "box.execute", "Please select a backup to execute." },
            { "box.execute_success", "executed in" },
            { "box.restore", "Please select a backup to restore." },
            { "box.restore_success", "restored !" },
            { "box.success", "Success" },
            { "box.logs", "Please select a log to open." },
            { "box.logs_exist", "Log file does not exist." },
            { "box.os", "Unsupported operating system." },
            { "box.logs_error", "Error opening file" },
            { "box.logs_format", "Logs format changed to" },
            { "menu.encrypt", "Please enter the encryption key for" },
            { "menu.decrypt", "Please enter the decryption key for" },
            { "menu.encryption_key", "Encryption Key" },
            { "menu.decryption_key", "Decryption Key" },
            { "menu.exception_application","Warning: business apps hasn't been added to the backup" },
            { "settings.valid_exe", "Please select a valid .exe file." },
            { "settings.application_added", "Application already added" },
            { "settings.application_added_success", "Application added successfully!" },
            { "settings.select_path_first", "Please select an application path first." },
            { "settings.add_business_process", "Add a business process" },
            { "settings.back", "Back" },
            { "settings.manage_business_processes", "Manage business processes" },
            { "settings.select_application_file", "Select an application file:" },
            { "settings.list_business_processes", "List of business processes:" },
        };
    }
}
