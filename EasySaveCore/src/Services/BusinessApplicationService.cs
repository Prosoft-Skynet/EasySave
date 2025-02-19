namespace EasySaveCore.src.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class BusinessApplicationService
{
    public List<string> BusinessApplications = new List<string>();

    public BusinessApplicationService() { }

    // Get the business applications from the JSON file
    public ObservableCollection<string> GetBusinessApplications()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BusinessApplications.json");
        var jsonContent = "";

        if (File.Exists(jsonPath))
        {
            jsonContent = File.ReadAllText(jsonPath);
            BusinessApplications = JsonSerializer.Deserialize<List<string>>(jsonContent) ?? new List<string>();
            return new ObservableCollection<string>(BusinessApplications);
        }
        else
        {
            var newJsonFile = File.Create(jsonPath);
            var jsonToUpdate = JsonSerializer.Serialize(new List<string>());

            newJsonFile.Write(Encoding.UTF8.GetBytes(jsonToUpdate));
            newJsonFile.Close();
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
}
