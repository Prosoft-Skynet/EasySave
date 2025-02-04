using System;
using System.Collections.Generic;

namespace EasySaveNamespace.Language
{
    public class LanguageManager
    {
        private Language currentLanguage = Language.French;
        private Dictionary<string, Dictionary<Language, string>> translations = new Dictionary<string, Dictionary<Language, string>>()
        {
            {"menu_title", new Dictionary<Language, string> { { Language.French, "--- Menu EasySave ---" }, { Language.English, "--- EasySave Menu ---" } }},
            {"add_backup", new Dictionary<Language, string> { { Language.French, "Ajouter une sauvegarde" }, { Language.English, "Add a backup" } }},
            {"delete_backup", new Dictionary<Language, string> { { Language.French, "Supprimer une sauvegarde" }, { Language.English, "Delete a backup" } }},
            {"list_backups", new Dictionary<Language, string> { { Language.French, "Lister les sauvegardes" }, { Language.English, "List backups" } }},
            {"execute_backup", new Dictionary<Language, string> { { Language.French, "Exécuter une sauvegarde" }, { Language.English, "Execute a backup" } }},
            {"restore_backup", new Dictionary<Language, string> { { Language.French, "Restaurer une sauvegarde" }, { Language.English, "Restore a backup" } }},
            {"view_logs", new Dictionary<Language, string> { { Language.French, "Voir les logs" }, { Language.English, "View logs" } }},
            {"view_state", new Dictionary<Language, string> { { Language.French, "Voir l'état" }, { Language.English, "View state" } }},
            {"change_language", new Dictionary<Language, string> { { Language.French, "FR/EN" }, { Language.English, "FR/EN" } }},
            {"quit", new Dictionary<Language, string> { { Language.French, "Quitter" }, { Language.English, "Quit" } }},
            {"choose_option", new Dictionary<Language, string> { { Language.French, "Votre choix: " }, { Language.English, "Your choice: " } }},
            {"invalid_option", new Dictionary<Language, string> { { Language.French, "Option invalide." }, { Language.English, "Invalid option." } }},
            {"press_enter", new Dictionary<Language, string> { { Language.French, "Appuyez sur Entrée pour continuer..." }, { Language.English, "Press Enter to continue..." } }},
            {"Language_changed", new Dictionary<Language, string> { { Language.French, "Langue changée en" }, { Language.English, "Language changed to" } }},
            {"Language", new Dictionary<Language, string> { { Language.French, "Français" }, { Language.English, "English" } }},
        };

        public string GetTranslation(string key) => translations[key][currentLanguage];

        public void ChangeLanguage()
        {
            currentLanguage = currentLanguage == Language.French ? Language.English : Language.French;
            Console.WriteLine($"{GetTranslation("Language_changed")} {GetTranslation("Language")}");
        }
    }

    public enum Language { French, English }
}
