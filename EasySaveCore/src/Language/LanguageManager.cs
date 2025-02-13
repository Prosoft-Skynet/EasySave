namespace EasySaveCore.Language;

/// <summary>
/// Manages the current language and associated translations.
/// </summary>
public class LanguageManager
{
    /// <summary>
    /// Language currently selected.
    /// </summary>
    private Language currentLanguage = null!;

    /// <summary>
    /// Dictionary containing translations in key/value form.
    /// </summary>
    private Dictionary<string, string> translations = new Dictionary<string, string>();

    /// <summary>
    /// Initializes a new LanguageManager object with a given language.
    /// </summary>
    /// <param name="language">Language to be used.</param>
    public LanguageManager(Language language)
    {
        SetLanguage(language);
    }

    /// <summary>
    /// Defines the active language and loads its translations.
    /// </summary>
    /// <param name="language">New language to apply.</param>
    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        translations = currentLanguage.GetTranslations();
    }

    /// <summary>
    /// Retrieves the translation associated with a given key.
    /// </summary>
    /// <param name="key">Key to the desired translation.</param>
    /// <returns>The corresponding translation if available, otherwise an error message.</returns>
    public string GetText(string key)
    {
        return translations.ContainsKey(key) ? translations[key] : $"Clé introuvable: {key}";
    }
}

