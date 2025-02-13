using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace EasySaveGUI.ViewModels
{
    public class MainWindowViewModel
    {
        // Liste des sauvegardes
        public ObservableCollection<string> Backups { get; } = new ObservableCollection<string>();

        // Commandes pour les boutons
        public ICommand AddBackupCommand { get; }
        public ICommand DeleteBackupCommand { get; }
        public ICommand RunBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }
        public ICommand ViewLogsCommand { get; }
        public ICommand ViewStateCommand { get; }
        public ICommand ToggleLanguageCommand { get; }
        public ICommand ExitCommand { get; }

        private bool isFrench = true;

        public MainWindowViewModel()
        {
            AddBackupCommand = new RelayCommand(AddBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup);
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
            MessageBox.Show($"Ajouté : {backupName}");
        }

        private void DeleteBackup()
        {
            if (Backups.Count > 0)
            {
                string removedBackup = Backups[^1];
                Backups.RemoveAt(Backups.Count - 1);
                MessageBox.Show($"Supprimé : {removedBackup}");
            }
            else
            {
                MessageBox.Show("Aucune sauvegarde à supprimer.");
            }
        }

        private void RunBackup() => MessageBox.Show("Exécution d'une sauvegarde...");
        private void RestoreBackup() => MessageBox.Show("Restauration d'une sauvegarde...");
        private void ViewLogs() => MessageBox.Show("Affichage des logs...");
        private void ViewState() => MessageBox.Show("Affichage de l'état...");

        private void ToggleLanguage()
        {
            isFrench = !isFrench;
            MessageBox.Show(isFrench ? "Langue : Français" : "Language: English");
        }

        private void ExitApplication() => Application.Current.Shutdown();
    }
}
