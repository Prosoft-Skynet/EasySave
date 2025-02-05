namespace EasySaveConsole.EasySaveNamespace.Language;

public class LanguageManager
{
    private Language currentLanguage = null!;
    private Dictionary<string, string> translations = new Dictionary<string, string>();

    public LanguageManager(Language language)
    {
        SetLanguage(language);
    }
    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        translations = currentLanguage.GetTranslations();
    }

    public string GetText(string key)
    {
        return translations.ContainsKey(key) ? translations[key] : $"Clé introuvable: {key}";
    }
}

