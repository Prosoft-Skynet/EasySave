using System;
using System.Collections.Generic;
using System.Text.Json;
using EasySaveCore.src.Models;

namespace EasySaveCore.src.Services
{
    public class LanguageService
    {

        private string currentLanguage = "en";
        private List<WordModel>? translations;

        public LanguageService()
        {
            GetJsonWord();
        }

        //C:\\Users\\thoma\\Desktop\\CESI_3A\\Genie_logiciel\\projet_final\\EasySaveGUI\\Assets\\JSONs\\words.json
        //C:\Users\thoma\Desktop\CESI_3A\Genie_logiciel\projet_final\EasySaveGUI\Assets\JSONs\words.json
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
            Console.WriteLine($"{GetTranslation("Language_changed")} {GetTranslation("Language")}");
        }
    }

}
