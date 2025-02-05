namespace EasySaveConsole.EasySaveNamespace.Language;

/// <summary>
/// Gère la langue actuelle et les traductions associées.
/// </summary>
public class LanguageManager
{
    /// <summary>
    /// Langue actuellement sélectionnée.
    /// </summary>
    private Language currentLanguage = null!;

    /// <summary>
    /// Dictionnaire contenant les traductions sous forme clé/valeur.
    /// </summary>
    private Dictionary<string, string> translations = new Dictionary<string, string>();

    /// <summary>
    /// Initialise un nouvel objet LanguageManager avec une langue donnée.
    /// </summary>
    /// <param name="language">Langue à utiliser.</param>
    public LanguageManager(Language language)
    {
        SetLanguage(language);
    }

    /// <summary>
    /// Définit la langue active et charge ses traductions.
    /// </summary>
    /// <param name="language">Nouvelle langue à appliquer.</param>
    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        translations = currentLanguage.GetTranslations();
    }

    /// <summary>
    /// Récupère la traduction associée à une clé donnée.
    /// </summary>
    /// <param name="key">Clé de la traduction recherchée.</param>
    /// <returns>La traduction correspondante si elle existe, sinon un message d'erreur.</returns>
    public string GetText(string key)
    {
        return translations.ContainsKey(key) ? translations[key] : $"Clé introuvable: {key}";
    }
}

