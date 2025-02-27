namespace EasySaveCore.src.Services;

using System.Threading;
using System;
using System.IO;

/// <summary>
/// Singleton class for file encryption.
/// </summary>
public class CryptoSoftService
{
    private static readonly Mutex mutex = new Mutex();
    private static readonly CryptoSoftService instance = new CryptoSoftService();

    private CryptoSoftService() { }

    public static CryptoSoftService Instance
    {
        get
        {
            return instance;
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
            Console.WriteLine("Another instance of the program is already running.");
            Environment.Exit(-2);
        }

        try
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Error : missing arguments");
                Console.WriteLine("Syntax : EasySaveCore crypt <file path> <encryption key>");
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
