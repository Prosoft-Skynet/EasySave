namespace EasySaveCore.src.Services;

using System;
using System.Collections.Generic;
using System.Text.Json;
using EasySaveCore.src.Models;

public class LanguageService
{
    private string currentLanguage = "en";
    private List<WordModel>? translations;

    public LanguageService()
    {
        try
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language.txt");
            if (File.Exists(filePath))
            {
                currentLanguage = File.ReadAllText(filePath);
            }
            else
            {
                currentLanguage = "en";
                File.WriteAllText(filePath, currentLanguage);
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Erreur : {ex.Message}");
        }

        GetJsonWord();
    }

    private void GetJsonWord()
    {
        string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        string filePath = Path.Combine(projectDirectory, "Assets", "JSONs", "words.json");
        List<WordModel> words = new List<WordModel>();

        if (File.Exists(filePath))
        {
            try
            {
                // Lire le JSON depuis le fichier
                string jsonString = File.ReadAllText(filePath);

                // Désérialiser en liste d'objets C#
                translations = JsonSerializer.Deserialize<List<WordModel>>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Le fichier JSON n'existe pas.");
        }
    }

    public string GetTranslation(string key)
    {
        if (translations == null)
        {
            GetJsonWord();
        }
        if (translations != null)
        {
            foreach (var translation in translations)
            {
                if (translation.Title == key)
                {
                    switch (currentLanguage)
                    {
                        case "fr":
                            return translation.Fr;
                        case "en":
                            return translation.En;
                        default:
                            break;
                    }
                }
            }
        }
        return "";
    }

    public void ChangeLanguage()
    {
        currentLanguage = currentLanguage == "fr" ? "en" : "fr";

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language.txt");
        File.WriteAllText(filePath, currentLanguage);

        Console.WriteLine($"{GetTranslation("Language_changed")} {GetTranslation("Language")}");
    }
}
