using System.Threading;

namespace EasySaveCore.src.Services;

using System;
using System.IO;

/// <summary>
/// Singleton class for file encryption.
/// </summary>
public class CryptoSoftService
{
    private static readonly Mutex mutex = new Mutex(true, "{B1AFC9E1-8C3D-4A6B-9A1A-2D3E4F5A6B7C}");
    private static CryptoSoftService instance = null;
    private static readonly object padlock = new object();

    private CryptoSoftService() { }

    public static CryptoSoftService Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new CryptoSoftService();
                }
                return instance;
            }
        }
    }

    /// <summary>
    /// Main method for encrypting a file.
    /// </summary>
    /// <param name="args">Array of strings containing the file path and encryption key.</param>
    public void Crypt(string[] args)
    {
        if (!mutex.WaitOne(TimeSpan.Zero, true))
        {
            Console.WriteLine("Une instance de CryptoSoft est déjà en cours d'exécution.");
            Environment.Exit(-2);
        }

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

            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\EasySaveConsole\bin\Debug\net8.0\");

            Console.WriteLine($"File: {fullPath}");

            string filePath = Path.GetFullPath(Path.Combine(baseDirectory, fullPath));

            Console.WriteLine($"Encryption key : {key}");

            var fileManager = new FileService(filePath, key);
            long elapsedTime = fileManager.TransformFile();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception : " + e.Message);
            Environment.Exit(-99);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }
}
