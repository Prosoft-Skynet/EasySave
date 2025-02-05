using System;
using System.Collections.Generic;
using System.Text.Json;
using EasySaveConsole.Language;

namespace EasySaveNamespace.Language
{
    public class LanguageManager
    {

        private string currentLanguage = "fr";
        private List<Word>? translations;


        //C:\\Users\\thoma\\Desktop\\CESI_3A\\Genie_logiciel\\projet_final\\EasySaveConsole\\bin\\Debug\\Assets\\JSONs\\words.json
        //C:\\Users\\thoma\\Desktop\\CESI_3A\\Genie_logiciel\\projet_final\\EasySaveConsole\\bin\\Assets\\JSONs\\words.json
        public void GetJsonWord()
        {
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string filePath = Path.Combine(projectDirectory, "Assets", "JSONs", "words.json");
            List<Word> words = new List<Word>();

            if (File.Exists(filePath))
            {
                try
                {
                    // Lire le JSON depuis le fichier
                    string jsonString = File.ReadAllText(filePath);
                    
                    // Désérialiser en liste d'objets C#
                    this.translations = JsonSerializer.Deserialize<List<Word>>(jsonString);
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



        public string GetTranslation(string key) {
            if (this.translations == null) { 
                this.GetJsonWord();
            }
            if (translations != null)
            {
                foreach (var translation in this.translations)
                {
                    if (translation.Title == key)
                    {
                        switch (this.currentLanguage)
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
            this.currentLanguage = this.currentLanguage == "fr" ? "en" : "fr";
            Console.WriteLine($"{GetTranslation("Language_changed")} {GetTranslation("Language")}");
        }
    }

}
