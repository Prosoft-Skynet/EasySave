using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using EasySaveGUI.Helpers;

namespace EasySaveGUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool isFrench = true;
        private string _statusMessage = "Prêt"; // Initialisation correcte

        public ObservableCollection<string> Backups { get; } = new ObservableCollection<string>();

        public ICommand AddBackupCommand { get; }
        public ICommand DeleteBackupCommand { get; }
        public ICommand RunBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }
        public ICommand ViewLogsCommand { get; }
        public ICommand ViewStateCommand { get; }
        public ICommand ToggleLanguageCommand { get; }
        public ICommand ExitCommand { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            AddBackupCommand = new RelayCommand(AddBackup);
            DeleteBackupCommand = new RelayCommand<string>(DeleteBackup, CanDeleteBackup);
            RunBackupCommand = new RelayCommand(RunBackup);
            RestoreBackupCommand = new RelayCommand(RestoreBackup);
            ViewLogsCommand = new RelayCommand(ViewLogs);
            ViewStateCommand = new RelayCommand(ViewState);
            ToggleLanguageCommand = new RelayCommand(ToggleLanguage);
            ExitCommand = new RelayCommand(ExitApplication);
        }

        private void AddBackup()
        {
            string backupName = $"Sauvegarde {Backups.Count + 1}";
            Backups.Add(backupName);
            StatusMessage = $"Ajouté : {backupName}";
        }

        private bool CanDeleteBackup(string? backup) => backup != null && Backups.Contains(backup);

        private void DeleteBackup(string backup)
        {
            Backups.Remove(backup);
            StatusMessage = $"Supprimé : {backup}";
        }

        private void RunBackup() => StatusMessage = "Exécution d'une sauvegarde...";
        private void RestoreBackup() => StatusMessage = "Restauration d'une sauvegarde...";
        private void ViewLogs() => StatusMessage = "Affichage des logs...";
        private void ViewState() => StatusMessage = "Affichage de l'état...";

        private void ToggleLanguage()
        {
            isFrench = !isFrench;
            StatusMessage = isFrench ? "Langue : Français" : "Language: English";
        }

        private void ExitApplication() => Application.Current.Shutdown();

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
