using EasySaveNamespace.Language;

namespace EasySaveNamespace.CLI
{
    public class Program
    {
        private LanguageManager _languageManager = new LanguageManager();

        public void Start()
        {
            var isChoiceMade = false;
            while (!isChoiceMade)
            {
                
                Console.WriteLine($"\n--- {_languageManager.GetTranslation("menu_title")} ---");
                Console.WriteLine($"1. {_languageManager.GetTranslation("add_backup")}");
                Console.WriteLine($"2. {_languageManager.GetTranslation("delete_backup")}");
                Console.WriteLine($"3. {_languageManager.GetTranslation("list_backups")}");
                Console.WriteLine($"4. {_languageManager.GetTranslation("execute_backup")}");
                Console.WriteLine($"5. {_languageManager.GetTranslation("restore_backup")}");
                Console.WriteLine($"6. {_languageManager.GetTranslation("view_logs")}");
                Console.WriteLine($"7. {_languageManager.GetTranslation("view_state")}");
                
                Console.Write("Choix : ");
                string choice = Console.ReadLine();
                isChoiceMade = true;
                switch (choice)
                {
                    case "1":
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "6":
                        break;
                    case "7":
                        break;
                    default:
                        Console.WriteLine("ce choix n'est pas disponible");
                        isChoiceMade = false;
                        break;
                }
            }
        }
        static void Main()
        {
            Program Interface = new Program();
            Interface.Start();
        }
    }

}
