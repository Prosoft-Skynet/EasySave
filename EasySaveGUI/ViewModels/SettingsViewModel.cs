namespace EasySaveGUI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using EasySaveGUI.Helpers;
    using EasySaveLogger.Logger;
    using EasySaveGUI.Views;
    using EasySaveCore.src.Services;
    using EasySaveCore.src.Services.BackupJobServices;

    public class SettingsViewModel : ViewModelBase
    {
        private string _businessApplicationPath = string.Empty;
        private readonly BackupService _backubService;

        private ObservableCollection<string> _businessApplication;
        private LanguageService _languageService;
        private BusinessApplicationService _businessApplicationService;
        public ICommand RemoveExtensionCommand { get; }

        private ObservableCollection<string> _userExtensionsToEncrypt = new ObservableCollection<string>();
        public ObservableCollection<string> UserExtensionsToEncrypt
        {
            get => _userExtensionsToEncrypt;
            set
            {
                _userExtensionsToEncrypt = value;
                OnPropertyChanged();
            }
        }

        private string? _selectedBusinessApplication;
        public string? SelectedBusinessApplication
        {
            get => _selectedBusinessApplication;
            set
            {
                _selectedBusinessApplication = value;
                OnPropertyChanged();
            }
        }

        private string? _selectedExtensionToRemove;
        public string? SelectedExtensionToRemove
        {
            get => _selectedExtensionToRemove;
            set
            {
                _selectedExtensionToRemove = value;
                OnPropertyChanged();
            }
        }

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
        public ICommand AddExtensionCommand { get; }
        public ICommand RemoveBusinessApplicationCommand { get; }

        public string this[string key] => _languageService.GetTranslation(key);

        public SettingsViewModel()
        {
            _languageService = new LanguageService();
            _backubService = new BackupService();
            _businessApplicationService = new BusinessApplicationService();
            _businessApplication = new ObservableCollection<string>(_businessApplicationService.GetBusinessApplications());

            // Load existing extensions into the list
            foreach (var extension in _backubService.LoadExtensionsToEncrypt())
            {
                if (!UserExtensionsToEncrypt.Contains(extension))
                {
                    UserExtensionsToEncrypt.Add(extension);
                }
            }

            SelectBusinessApplicationCommand = new RelayCommand(SelectBusinessApplication);
            CloseSettingsCommand = new RelayCommand(CloseSettings);
            AddBusinessApplicationCommand = new RelayCommand(AddBusinessApplication);
            RemoveExtensionCommand = new RelayCommand(RemoveExtension);
            AddExtensionCommand = new RelayCommand(AddExtension);
            RemoveBusinessApplicationCommand = new RelayCommand(RemoveBusinessApplication);
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

        private void AddExtension()
        {
            var input = CustomInputDialog.ShowDialog(
                _languageService.GetTranslation("extension.ajout"),
                _languageService.GetTranslation("extension.info")
            );

            if (!string.IsNullOrWhiteSpace(input))
            {
                var extensions = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(ext => ext.Trim())
                                       .Where(ext => ext.StartsWith(".") && ext.Length > 1)
                                       .Distinct()
                                       .ToList();

                foreach (var extension in extensions)
                {
                    if (!UserExtensionsToEncrypt.Contains(extension))
                    {
                        UserExtensionsToEncrypt.Add(extension);
                        _backubService.AddExtensionToEncrypt(extension);
                        MessageBox.Show($"{_languageService.GetTranslation("extension.valid")} {extension}");
                    }
                    else
                    {
                        MessageBox.Show($"{_languageService.GetTranslation("extension.present")} {extension}");
                    }
                }

                if (extensions.Count == 0)
                {
                    MessageBox.Show(_languageService.GetTranslation("extension.none"));
                }
            }
            else
            {
                MessageBox.Show(_languageService.GetTranslation("exec.extension"));
            }
        }

        private void RemoveExtension()
        {
            if (UserExtensionsToEncrypt.Count == 0 || SelectedExtensionToRemove == null)
            {
                MessageBox.Show(_languageService.GetTranslation("extension.no_selection"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var extensionToRemove = SelectedExtensionToRemove;

            if (UserExtensionsToEncrypt.Contains(extensionToRemove))
            {
                UserExtensionsToEncrypt.Remove(extensionToRemove);
                _backubService.RemoveExtensionToEncrypt(extensionToRemove);
                MessageBox.Show($"{_languageService.GetTranslation("extension.removed")} {extensionToRemove}");
            }
            else
            {
                MessageBox.Show(_languageService.GetTranslation("extension.not_found"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveBusinessApplication()
        {
            if (BusinessApplications.Count == 0 || SelectedBusinessApplication == null)
            {
                MessageBox.Show(_languageService.GetTranslation("settings_remove.no_selection"),
                                _languageService.GetTranslation("box.error"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var appToRemove = SelectedBusinessApplication;

            if (BusinessApplications.Contains(appToRemove))
            {
                BusinessApplications.Remove(appToRemove);

                _businessApplicationService.RemoveBusinessApplication(appToRemove);

                MessageBox.Show($"{_languageService.GetTranslation("settings_remove.application_remove")} {appToRemove}",
                                "Information",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(_languageService.GetTranslation("settings_remove.application_not_found"),
                                _languageService.GetTranslation("box.error"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }

        private void CloseSettings()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is Views.SettingsWindow)?.Close();
        }
    }
}
