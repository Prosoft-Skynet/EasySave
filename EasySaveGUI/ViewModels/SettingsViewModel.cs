namespace EasySaveGUI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using EasySaveGUI.Helpers;
using EasySaveLogger.Logger;
using System.Runtime.InteropServices;
using EasySaveGUI.Views;
using EasySaveCore.src.Services;
using EasySaveCore.src.Models;
using EasySaveCore.src.Services.BackupJobServices;

public class SettingsViewModel : ViewModelBase
{
  
    private string _businessApplicationPath = string.Empty;
    private ObservableCollection<string> _businessApplication;
    private LanguageService _languageService;
    private BusinessApplicationService _businessApplicationService;




    public string BusinessApplicationsPath
    {
        get => _businessApplicationPath;
        set { _businessApplicationPath = value; OnPropertyChanged(); }
    }
    public ObservableCollection<string> BusinessApplications
    {
        get => _businessApplication;
        set { _businessApplication = value; OnPropertyChanged(); }
    }



    public ICommand SelectBusinessApplicationCommand { get; }
    public ICommand CloseSettingsCommand { get; }
    public ICommand AddBusinessApplicationCommand { get; }

    public string this[string key] => _languageService.GetTranslation(key);


    public SettingsViewModel()
    {
        _languageService = new LanguageService();
        _businessApplicationService = new BusinessApplicationService();
        _businessApplication = new ObservableCollection<string>(_businessApplicationService.GetBusinessApplications());

        SelectBusinessApplicationCommand = new RelayCommand(SelectBusinessApplication);
        CloseSettingsCommand = new RelayCommand(CloseSettings);
        AddBusinessApplicationCommand = new RelayCommand(AddBusinessApplication);


    }


    private void SelectBusinessApplication()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Sélectionnez une application",
            Filter = "Fichiers exécutables (*.exe)|*.exe",
            CheckFileExists = true,
            CheckPathExists = true
        };

        if (dialog.ShowDialog() == true)
        {
            string selectedPath = dialog.FileName;

            // Vérifier si le fichier est bien un .exe
            if (System.IO.Path.GetExtension(selectedPath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                BusinessApplicationsPath = selectedPath;
            }
            else
            {
                MessageBox.Show($"{_languageService.GetTranslation("settings.valid_exe")}", _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void AddBusinessApplication()
    {
        if (!string.IsNullOrWhiteSpace(BusinessApplicationsPath))
        {
            if (System.IO.Path.GetExtension(BusinessApplicationsPath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                if (!BusinessApplications.Contains(BusinessApplicationsPath))
                {
                    _businessApplicationService.AddBusinessApplication(BusinessApplicationsPath);
                    _businessApplication.Add(BusinessApplicationsPath);
                    BusinessApplicationsPath = string.Empty;
                    MessageBox.Show($"{_languageService.GetTranslation("settings.application_added_success")}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"{_languageService.GetTranslation("settings.application_added")}", _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show($"{_languageService.GetTranslation("settings.valid_exe")}", _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        else
        {
            MessageBox.Show($"{_languageService.GetTranslation("settings.select_path_first")}", _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void CloseSettings()
    {
        Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w is Views.SettingsWindow)?.Close();
    }

}
