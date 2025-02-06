namespace EasySaveConsole.EasySaveNamespace.State;

/// <summary>
/// Représente l'état d'un travail de sauvegarde.
/// Contient des informations sur le job telles que le nom, le statut, la progression, la taille et les fichiers restants.
/// </summary>
public class StateEntry
{
    /// <summary>
    /// Nom du job de sauvegarde.
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    /// Horodatage indiquant quand l'état du job a été enregistré.
    /// </summary>
    public string Timestamp { get; set; }

    /// <summary>
    /// Statut actuel du job (ex : "Actif", "Terminé", etc.).
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Nombre total de fichiers à sauvegarder.
    /// </summary>
    public int FilesTotal { get; set; }

    /// <summary>
    /// Taille totale des fichiers à sauvegarder (en octets).
    /// </summary>
    public int SizeTotal { get; set; }

    /// <summary>
    /// Progression du job, entre 0 (aucune progression) et 1 (complètement terminé).
    /// </summary>
    public float Progress { get; set; }

    /// <summary>
    /// Nombre de fichiers restants à traiter.
    /// </summary>
    public int RemainingFiles { get; set; }

    /// <summary>
    /// Taille des fichiers restants à traiter (en octets).
    /// </summary>
    public int RemainingSize { get; set; }

    /// <summary>
    /// Source actuelle du travail de sauvegarde (répertoire ou fichier source).
    /// </summary>
    public string CurrentSource { get; set; }

    /// <summary>
    /// Destination actuelle de la sauvegarde (répertoire cible).
    /// </summary>
    public string CurrentTarget { get; set; }

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="StateEntry"/>.
    /// Le statut par défaut est "Non Actif" et l'horodatage est défini à la date et l'heure actuelles.
    /// </summary>
    public StateEntry()
    {
        JobName = string.Empty;
        CurrentSource = string.Empty;
        CurrentTarget = string.Empty;
        Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Status = "Non Actif";
    }
}