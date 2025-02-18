namespace EasySaveCore.Language;

/// <summary>
/// Implementation of the Language class for the French language.
/// </summary>
public class FrLanguage : Language
{
    /// <summary>
    /// Returns a dictionary containing French translations.
    /// </summary>
    /// <returns>Dictionary of translation keys with their French text.</returns>
    public override Dictionary<string, string> GetTranslations()
    {
        return new Dictionary<string, string>
        {
            { "menu.add", "Ajouter une sauvegarde" },
            { "menu.delete", "Supprimer une sauvegarde" },
            { "menu.execute", "Exécuter une sauvegarde" },
            { "menu.restore", "Restaurer une sauvegarde" },
            { "menu.logs", "Voir les logs" },
            { "menu.state", "Voir l'état" },
            { "menu.create", "Créer une nouvelle sauvegarde" },
            { "menu.name", "Nom de la sauvegarde :" },
            { "menu.source", "Dossier source :" },
            { "menu.target", "Dossier cible :" },
            { "menu.type", "Type de sauvegarde :" },
            { "menu.complete", "Complète" },
            { "menu.differential", "Différentielle" },
            { "menu.list", "Liste des sauvegardes :" },
            { "menu.params", "Paramètres" },
            { "menu.quit", "Quitter" },
            { "menu.change_logs", "Changer format Logs (XML/JSON)" },
            { "box.error", "Erreur" },
            { "box.fill", "Veuillez remplir tous les champs." },
            { "box.name", "Une sauvegarde avec ce nom existe déjà." },
            { "box.files", "Sélectionnez un dossier" },
            { "box.backup", "Sauvegarde" },
            { "box.create_success", "créée avec succès !" },
            { "box.delete", "Sélectionnez une sauvegarde à supprimer." },
            { "box.delete_success", "supprimée avec succès !" },
            { "box.execute", "Veuillez sélectionner une sauvegarde à exécuter." },
            { "box.execute_success", "exécutée en" },
            { "box.restore", "Veuillez sélectionner une sauvegarde à restaurer." },
            { "box.restore_success", "restaurée !" },
            { "box.success", "Succès" },
            { "box.logs", "Veuillez sélectionner un log à ouvrir." },
            { "box.logs_exist", "Le fichier log n'existe pas." },
            { "box.os", "Système d'exploitation non supporté." },
            { "box.logs_error", "Erreur lors de l'ouverture du fichier" },
            { "box.logs_format", "Format des logs changé en" },
            { "menu.encrypt", "Veuillez entrer la clé de cryptage pour" },
            { "menu.decrypt", "Veuillez entrer la clé de décryptage pour" },
            { "menu.encryption_key", "Clé de cryptage" },
            { "menu.decryption_key", "Clé de décryptage" },
            { "menu.exception_application","Attention : des applications métiers n'ont pas été ajoutées à la sauvegarde." },
            { "settings.valid_exe","Veuillez sélectionner un fichier .exe valide." },
            { "settings.application_added","Application déjà ajoutée" },
            { "settings.application_added_success","Application ajoutée avec succès !" },
            { "settings.select_path_first", "Veuillez d'abord sélectionner un chemin d'application." },
            { "settings.add_business_process","Ajouter un processus métier" },
            { "settings.back","Retour" },
            { "settings.manage_business_processes","Gérer les processus métiers" },
            { "settings.select_application_file","Sélectionner un fichier d'application :" },
            { "settings.list_business_processes","Liste des processus métiers :" },
        };
    }
}
