using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace EasySaveCore.CryptoSoft;

/// <summary>
/// File manager class
/// This class is used to encrypt and decrypt files
/// </summary>
public class FileManager
{
    private string FilePath { get; }
    private string Key { get; }

    public FileManager(string path, string key)
    {
        FilePath = path;
        Key = key;
    }

    /// <summary>
    /// Check if the file exists
    /// </summary>
    private bool CheckFile()
    {
        if (File.Exists(FilePath))
            return true;

        Console.WriteLine("Erreur : Fichier non trouvé.");
        return false;
    }

    /// <summary>
    /// Encrypts or decrypts the file with XOR encryption
    /// </summary>
    public int TransformFile()
    {
        if (!CheckFile()) return -1;

        Stopwatch stopwatch = Stopwatch.StartNew();
        var fileBytes = File.ReadAllBytes(FilePath);
        var keyBytes = ConvertToByte(Key);
        fileBytes = XorMethod(fileBytes, keyBytes);
        File.WriteAllBytes(FilePath, fileBytes);
        stopwatch.Stop();

        // Vérification si le fichier a bien été transformé
        if (fileBytes.Length > 0)
        {
            // Message de confirmation après chiffrement ou déchiffrement
            Console.WriteLine("");
        }
        else
        {
            Console.WriteLine("Erreur dans le processus de transformation du fichier.");
        }

        return (int)stopwatch.ElapsedMilliseconds;
    }

    /// <summary>
    /// Convert a string in byte array
    /// </summary>
    /// <param name="text">The string to convert</param>
    /// <returns>The byte array</returns>
    private static byte[] ConvertToByte(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    /// <summary>
    /// XOR encryption/decryption method
    /// </summary>
    /// <param name="fileBytes">Bytes of the file to transform</param>
    /// <param name="keyBytes">Key to use for XOR operation</param>
    /// <returns>Bytes of the transformed file</returns>
    private static byte[] XorMethod(IReadOnlyList<byte> fileBytes, IReadOnlyList<byte> keyBytes)
    {
        var result = new byte[fileBytes.Count];
        for (var i = 0; i < fileBytes.Count; i++)
        {
            result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Count]);
        }

        return result;
    }
}
