using System;

namespace EasySave.src
{
    public class Program
    {
        public void Start()

        {
            while (true)
            {
                Console.WriteLine("\n--- EasySave Menu ---");
                Console.WriteLine("1. Ajouter une sauvegarde");
                Console.WriteLine("2. Supprimer une sauvegarde");
                Console.WriteLine("3. Lister les sauvegardes");
                Console.WriteLine("4. Exécuter une sauvegarde");
                Console.WriteLine("5. Restaurer une sauvegarde");
                Console.WriteLine("6. Voir les logs");
                Console.WriteLine("7. Quitter");

                Console.Write("Choix : ");
                string choice = Console.ReadLine();

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
                        return;
                }
            }
        }
        static void Main()
        {
            Program interfaces = new Program();
            interfaces.Start();
        }
    }

}