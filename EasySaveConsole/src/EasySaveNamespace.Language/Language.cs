namespace EasySaveConsole.EasySaveNamespace.Language;

using System.Collections.Generic;

/// <summary>
/// Class representing a language and its translations.
/// </summary>
public abstract class Language
{
    /// <summary>
    /// Method for obtaining translations in dictionary form.
    /// </summary>
    /// <returns>A dictionary containing the translation keys and their associated values.</returns>
    public abstract Dictionary<string, string> GetTranslations();
}
