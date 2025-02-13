namespace EasySaveGUI.ViewModels;

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EasySaveCore.Backup;
using EasySaveCore.Controller;
using EasySaveGUI.Helpers;

public class MainViewModel : ViewModelBase
{
    private readonly BackupManager _backupManager;

    private string _backupName = string.Empty;
    private string _sourcePath = string.Empty;
    private string _destinationPath = string.Empty;
    private bool _isFullBackup = true;

    public ObservableCollection<BackupJob> Backups { get; }

    private BackupJob? _selectedBackup;
    public BackupJob? SelectedBackup
    {
        get => _selectedBackup;
        set
        {
            _selectedBackup = value;
            OnPropertyChanged();

            // Mise à jour de l'état des boutons
            OnPropertyChanged(nameof(CanExecuteOrRestoreOrDelete));
            OnPropertyChanged(nameof(CanAddBackup));
        }
    }

    // Propriété pour activer/désactiver les boutons "Exécuter" et "Restaurer"
    public bool CanExecuteOrRestoreOrDelete => SelectedBackup != null;

    // Propriété pour activer/désactiver le bouton "Ajouter"
    public bool CanAddBackup => SelectedBackup == null;


    public string BackupName
    {
        get => _backupName;
        set { _backupName = value; OnPropertyChanged(); }
    }

    public string SourcePath
    {
        get => _sourcePath;
        set { _sourcePath = value; OnPropertyChanged(); }
    }

    public string DestinationPath
    {
        get => _destinationPath;
        set { _destinationPath = value; OnPropertyChanged(); }
    }

    public bool IsFullBackup
    {
        get => _isFullBackup;
        set { _isFullBackup = value; OnPropertyChanged(); }
    }

    public ICommand AddBackupCommand { get; }
    public ICommand DeleteBackupCommand { get; }

    public ICommand RunBackupCommand { get; }

    public ICommand RestoreBackupCommand { get; }

    public ICommand SelectSourceCommand { get; }
    public ICommand SelectDestinationCommand { get; }

    public ICommand ExitCommand { get; }

    public MainViewModel()
    {
        _backupManager = new BackupManager();
        Backups = new ObservableCollection<BackupJob>(_backupManager.GetBackupJobs());

        AddBackupCommand = new RelayCommand(AddBackup, () => CanAddBackup);
        DeleteBackupCommand = new RelayCommand(DeleteBackup, () => CanExecuteOrRestoreOrDelete);
        RunBackupCommand = new RelayCommand(RunBackup, () => CanExecuteOrRestoreOrDelete);
        RestoreBackupCommand = new RelayCommand(RestoreBackup, () => CanExecuteOrRestoreOrDelete);
        SelectSourceCommand = new RelayCommand(SelectSource);
        SelectDestinationCommand = new RelayCommand(SelectDestination);
        ExitCommand = new RelayCommand(ExitApplication);
    }

    private void AddBackup()
    {
        if (string.IsNullOrWhiteSpace(BackupName) || string.IsNullOrWhiteSpace(SourcePath) || string.IsNullOrWhiteSpace(DestinationPath))
        {
            MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Backups.Any(b => b.Name == BackupName))
        {
            MessageBox.Show("Une sauvegarde avec ce nom existe déjà.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IBackupTypeStrategy strategy = IsFullBackup ? new CompleteBackupStrategy() : new DifferentialBackupStrategy();
            var job = new BackupJob(BackupName, SourcePath, DestinationPath, IsFullBackup, strategy);

            _backupManager.AddBackup(job);
            Backups.Add(job);

            MessageBox.Show($"La sauvegarde {BackupName} a été créée avec succès !");

            BackupName = string.Empty;
            SourcePath = string.Empty;
            DestinationPath = string.Empty;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanDeleteBackup()
    {
        return SelectedBackup != null;
    }

    private void DeleteBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show("Sélectionnez une sauvegarde à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string backupName = SelectedBackup.Name;

        var jobToRemove = _backupManager.GetBackupJobs().FirstOrDefault(b => b.Id == SelectedBackup.Id);
        if (jobToRemove != null)
        {
            _backupManager.GetBackupJobs().Remove(jobToRemove);
        }

        Backups.Remove(SelectedBackup);

        SelectedBackup = null;

        MessageBox.Show($"Sauvegarde {backupName} supprimée avec succès !");
    }

    private void RunBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show("Veuillez sélectionner une sauvegarde à exécuter.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _backupManager.ExecuteJob(SelectedBackup.Id);
        MessageBox.Show($"Sauvegarde {SelectedBackup.Name} exécutée !");
    }

    private void RestoreBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show("Veuillez sélectionner une sauvegarde à restaurer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _backupManager.RestoreJob(SelectedBackup.Id);
        MessageBox.Show($"Sauvegarde {SelectedBackup.Name} restaurée !");
    }

    private void SelectSource()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Sélectionnez un dossier",
            ValidateNames = false
        };

        if (dialog.ShowDialog() == true)
        {
            SourcePath = System.IO.Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
        }
    }

    private void SelectDestination()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Sélectionnez un dossier",
            ValidateNames = false
        };

        if (dialog.ShowDialog() == true)
        {
            DestinationPath = System.IO.Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
        }
    }

    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
