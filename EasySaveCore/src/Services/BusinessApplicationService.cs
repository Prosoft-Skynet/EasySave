namespace EasySaveCore.src.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

public class BusinessApplicationService
{
    public List<string> BusinessApplications = new List<string>();

    public BusinessApplicationService() { }

    public ObservableCollection<string> GetBusinessApplications()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BusinessApplications.json");

        if (!File.Exists(jsonPath))
        {
            File.WriteAllText(jsonPath, "[]");
            return new ObservableCollection<string>();
        }

        try
        {
            string jsonContent = File.ReadAllText(jsonPath);
            BusinessApplications = JsonSerializer.Deserialize<List<string>>(jsonContent) ?? new List<string>();
            return new ObservableCollection<string>(BusinessApplications);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur de lecture du fichier JSON : {ex.Message}");
            return new ObservableCollection<string>();
        }
    }

    public void AddBusinessApplication(string path)
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BusinessApplications.json");

        BusinessApplications.Add(path);
        var jsonToUpdate = JsonSerializer.Serialize(BusinessApplications);

        File.WriteAllText(jsonPath, jsonToUpdate);
    }
    public void RemoveBusinessApplication(string path)
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BusinessApplications.json");

        if (BusinessApplications.Contains(path))
        {
            BusinessApplications.Remove(path);
            var jsonToUpdate = JsonSerializer.Serialize(BusinessApplications, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, jsonToUpdate);
        }
    }
}
