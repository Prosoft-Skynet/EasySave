namespace EasySaveConsole.EasySaveNamespace.Language;

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
            { "menu.add", "1. Ajouter une sauvegarde" },
            { "menu.delete", "2. Supprimer une sauvegarde" },
            { "menu.list", "3. Lister les sauvegardes" },
            { "menu.execute", "4. Exécuter une sauvegarde" },
            { "menu.restore", "5. Restaurer une sauvegarde" },
            { "menu.logs", "6. Voir les logs" },
            { "menu.state", "7. Voir l'état" },
            { "menu.state.Contents", "Contenu de l'état" },
            { "menu.logs_format_JSON", "8. JSON" },
            { "menu.logs_format_XML", "8. XML" },
            { "menu.language", "9. EN" },
            { "menu.quit", "10. Quitter" },
            { "menu.choice", "Votre choix: " },
            { "backup.name", "Nom de la sauvegarde : " },
            { "backup.name_use", "Nom déjà utilisé" },
            { "backup.source", "Répertoire source : " },
            { "backup.destination", "Répertoire cible : " },
            { "backup.type", "Type de sauvegarde (1: complète, 2: différentielle) : " },
            { "backup.add", "Sauvegarde ajoutée !" },
            { "delete.name", "Entrez le nom de la sauvegarde à supprimer : " },
            { "delete.delete", "Sauvegarde supprimée !" },
            { "name.invalid", "Nom invalide." },
            { "list.list", "\nListe des sauvegardes :" },
            { "list.none", "Aucune sauvegarde trouvée." },
            { "list.name", "Nom : " },
            { "list.target", "Cible : " },
            { "list.complete", "Complète" },
            { "list.differential", "Différentielle" },
            { "exec.name", "Entrez le nom de la sauvegarde à exécuter : " },
            { "exec.launch", "Lancement de la sauvegarde " },
            { "exec.finish", "Sauvegarde terminée !" },
            { "restore.name", "Entrez le nom de la sauvegarde à restaurer : " },
            { "restore.restore", "Restauration de la sauvegarde " },
            { "restore.finish", "Restauration terminée !" },
            { "logs.chose", "choisis le log à exécuter" },
            { "logs.JSON_format", "Affichage des logs en JSON" },
            { "logs.XML_format", "Affichage des logs en XML" },
            { "logs.none", "il n'y a aucun log pour le moment" }
        };
    }
}

