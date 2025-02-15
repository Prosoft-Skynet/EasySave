using System;
using System.IO;

namespace EasySaveCore.CryptoSoft;
/// <summary>
/// Static class for file encryption.
/// </summary>
public static class CryptoSoft
    {
    /// <summary>
    /// Main method for encrypting a file.
    /// </summary>
    /// <param name="args">Array of strings containing the file path and encryption key.</param>
    public static void Crypt(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Erreur : Nombre d'arguments insuffisant.");
                    Console.WriteLine("Syntaxe : cryptosoft.exe fichier_a_crypter clé_de_cryptage");
                    Environment.Exit(-1);
                }

                string fullPath = args[0];
                string key = args[1];

                string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"..\..\..\..\EasySaveConsole\bin\Debug\net8.0\");

                Console.WriteLine($"File: {fullPath}");

                string filePath = Path.GetFullPath(Path.Combine(baseDirectory, fullPath));

                Console.WriteLine($"Encryption key : {key}");

                var fileManager = new FileManager(filePath, key);
                long elapsedTime = fileManager.TransformFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
                Environment.Exit(-99);
            }
        }
    }

