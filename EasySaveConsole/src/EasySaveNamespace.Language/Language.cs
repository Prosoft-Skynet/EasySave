namespace EasySaveConsole.EasySaveNamespace.Language;

using System.Collections.Generic;

/// <summary>
/// Classe représentant une langue et ses traductions.
/// </summary>
public abstract class Language
{
    /// <summary>
    /// Méthode permettant d'obtenir les traductions sous forme d'un dictionnaire.
    /// </summary>
    /// <returns>Un dictionnaire contenant les clés de traduction et leurs valeurs associées.</returns>
    public abstract Dictionary<string, string> GetTranslations();
}
