namespace EasySaveConsole.EasySaveNamespace.Language;

using System.Collections.Generic;

public abstract class Language
{
    public abstract Dictionary<string, string> GetTranslations();
}
