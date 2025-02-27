namespace EasySaveCore.src.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

/// <summary>
/// File manager class
/// This class is used to encrypt and decrypt files
/// </summary>
public class FileService
{
    private string filePath { get; }
    private string key { get; }

    public FileService(string path, string key)
    {
        this.filePath = path;
        this.key = key;
    }

    /// <summary>
    /// Check if the file exists
    /// </summary>
    private bool CheckFile()
    {
        if (File.Exists(filePath))
            return true;

        Console.WriteLine("Error: The file does not exist.");
        return false;
    }

    /// <summary>
    /// Encrypts or decrypts the file with XOR encryption
    /// </summary>
    public int TransformFile()
    {
        if (!CheckFile()) return -1;

        Stopwatch stopwatch = Stopwatch.StartNew();
        var fileBytes = File.ReadAllBytes(filePath);
        var keyBytes = ConvertToByte(key);
        fileBytes = XorMethod(fileBytes, keyBytes);
        File.WriteAllBytes(filePath, fileBytes);
        stopwatch.Stop();

        if (fileBytes.Length > 0)
        {
            Console.WriteLine("");
        }
        else
        {
            Console.WriteLine("Error: The file could not be transformed.");
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
