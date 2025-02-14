namespace EasySaveGUI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using EasySaveCore.Backup;
using EasySaveCore.Controller;
using EasySaveCore.CryptoSoft;
using EasySaveCore.Language;
using EasySaveGUI.Helpers;
using EasySaveLogger.Logger;
using System.Runtime.InteropServices;
using EasySaveGUI.Views;

public class MainViewModel : ViewModelBase
{
    private readonly BackupManager _backupManager;
    private readonly Logger _logger;
    private string _backupName = string.Empty;
    private string _sourcePath = string.Empty;
    private string _destinationPath = string.Empty;
    private bool _isFullBackup = true;
    private string _businessApplicationPath = string.Empty;
    private ObservableCollection<string> _businessApplication = new ObservableCollection<string>();

    public ObservableCollection<BackupJob> Backups { get; }
    public ObservableCollection<string> Logs { get; }
    private EasySave easySave = EasySave.GetInstance();


    private BackupJob? _selectedBackup;
    public BackupJob? SelectedBackup
    {
        get => _selectedBackup;
        set
        {
            _selectedBackup = value;
            OnPropertyChanged();
        }
    }

    private string? _selectedLog;
    public string? SelectedLog
    {
        get => _selectedLog;
        set
        {
            _selectedLog = value;
            OnPropertyChanged();
        }
    }

    private string _encryptionKey = string.Empty;
    public string EncryptionKey
    {
        get => _encryptionKey;
        set { _encryptionKey = value; OnPropertyChanged(); }
    }

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
    public ICommand OpenLogCommand { get; }
    public ICommand ToggleLogFormatCommand { get; }
    public ICommand ToggleLanguageCommand { get; }
    public ICommand SelectBusinessApplicationCommand { get; }
    public ICommand ExitCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand AddBusinessApplicationCommand { get; }


    public MainViewModel()
    {
        _backupManager = new BackupManager();
        _logger = new Logger(new JsonLogFormatter());
        Backups = new ObservableCollection<BackupJob>(_backupManager.GetBackupJobs());
        BusinessApplications = new ObservableCollection<string>(_businessApplication);

        Backups = new ObservableCollection<BackupJob>(_backupManager.LoadBackupJobs());
        Logs = new ObservableCollection<string>();

        LoadLogs();

        AddBackupCommand = new RelayCommand(AddBackup);
        DeleteBackupCommand = new RelayCommand(DeleteBackup);
        RunBackupCommand = new RelayCommand(RunBackup);
        RestoreBackupCommand = new RelayCommand(RestoreBackup);
        SelectSourceCommand = new RelayCommand(SelectSource);
        SelectDestinationCommand = new RelayCommand(SelectDestination);
        OpenLogCommand = new RelayCommand(OpenLog);
        ToggleLogFormatCommand = new RelayCommand(ToggleLogFormat);
        ToggleLanguageCommand = new RelayCommand(ToggleLanguage);
        SelectBusinessApplicationCommand = new RelayCommand(SelectBusinessApplication);
        ExitCommand = new RelayCommand(ExitApplication);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        AddBusinessApplicationCommand = new RelayCommand(AddBusinessApplication);

    }

    private void AddBackup()
    {
        if (string.IsNullOrWhiteSpace(BackupName) || string.IsNullOrWhiteSpace(SourcePath) || string.IsNullOrWhiteSpace(DestinationPath))
        {
            MessageBox.Show(easySave.GetText("box.fill"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Backups.Any(b => b.Name == BackupName))
        {
            MessageBox.Show(easySave.GetText("box.name"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IBackupTypeStrategy strategy = IsFullBackup ? new CompleteBackupStrategy() : new DifferentialBackupStrategy();
            var job = new BackupJob(BackupName, SourcePath, DestinationPath, IsFullBackup, strategy);
            job.BusinessApplications = BusinessApplications ?? new ObservableCollection<string>();
            _backupManager.AddBackup(job);
            _backupManager.SaveBackupJobs();

            Backups.Add(job);
            MessageBox.Show($"{easySave.GetText("box.backup")} {BackupName} {easySave.GetText("box.create_success")}");

            BackupName = string.Empty;
            SourcePath = string.Empty;
            DestinationPath = string.Empty;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{easySave.GetText("box.error")} : {ex.Message}", easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show(easySave.GetText("box.delete"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string backupName = SelectedBackup.Name;

        var jobToRemove = _backupManager.GetBackupJobs().FirstOrDefault(b => b.Id == SelectedBackup.Id);
        if (jobToRemove != null)
        {
            _backupManager.GetBackupJobs().Remove(jobToRemove);
        }

        Backups.Remove(SelectedBackup);
        _backupManager.SaveBackupJobs();

        SelectedBackup = null;

        MessageBox.Show($"{easySave.GetText("box.backup")} {backupName} {easySave.GetText("box.delete_success")}");
    }

    private async void RunBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show(easySave.GetText("box.execute"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var startTime = DateTime.Now;

        Func<string, string> getEncryptionKeyCallback = (fileName) =>
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (!_backupManager.extensionsToEncrypt.Contains(extension)) return string.Empty;

            string encryptionKey = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                do
                {
                    encryptionKey = CustomInputDialog.ShowDialog(
                        $"{easySave.GetText("menu.encrypt")} {fileName}",
                        easySave.GetText("menu.encryption_key"));
                } while (string.IsNullOrEmpty(encryptionKey));
            });

            return encryptionKey;
        };

        var copyCryptStartTime = DateTime.Now;
        var copyCryptTimeMs = 0L;

        try
        {
            await Task.Run(() =>
            {
                _backupManager.ExecuteJobinterface(SelectedBackup.Id, getEncryptionKeyCallback);
            });

            var copyCryptEndTime = DateTime.Now;
            copyCryptTimeMs = (long)(copyCryptEndTime - copyCryptStartTime).TotalMilliseconds;
        }
        catch (Exception)
        {
            copyCryptTimeMs = -1000;
        }

        var endTime = DateTime.Now;
        long totalDurationMs = (long)(endTime - startTime).TotalMilliseconds;

        _logger.Log(
            SelectedBackup.Name,
            SelectedBackup.Source,
            SelectedBackup.Target,
            totalDurationMs,
            copyCryptTimeMs
        );

        MessageBox.Show($"{easySave.GetText("box.backup")} {SelectedBackup.Name} {easySave.GetText("box.execute_success")} {totalDurationMs} ms !", easySave.GetText("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);

        LoadLogs();
    }

    private async void RestoreBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show(easySave.GetText("box.restore"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Func<string, string> getDecryptionKeyCallback = (fileName) =>
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (!_backupManager.extensionsToEncrypt.Contains(extension)) return string.Empty;

            string decryptionKey = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                do
                {
                    decryptionKey = CustomInputDialog.ShowDialog(
                        $"{easySave.GetText("menu.decrypt")} {fileName}",
                        easySave.GetText("menu.decryption_key"));
                } while (string.IsNullOrEmpty(decryptionKey));
            });

            return decryptionKey;
        };

        try
        {
            await Task.Run(() =>
            {
                _backupManager.RestoreJob(SelectedBackup.Id, getDecryptionKeyCallback);
            });

            MessageBox.Show($"{easySave.GetText("box.backup")} {SelectedBackup.Name} {easySave.GetText("box.restore_success")}",
                            easySave.GetText("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{easySave.GetText("box.error")} : {ex.Message}",
                            easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SelectSource()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = easySave.GetText("box.files"),
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
            FileName = easySave.GetText("box.files"),
            ValidateNames = false
        };

        if (dialog.ShowDialog() == true)
        {
            DestinationPath = System.IO.Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
        }
    }

    private void LoadLogs()
    {
        Logs.Clear();

        string logsPath = _logger.GetLogFormatter() is JsonLogFormatter
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "JSON")
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "XML");

        if (!Directory.Exists(logsPath))
            return;

        var logFiles = Directory.GetFiles(logsPath)
            .Select(Path.GetFileNameWithoutExtension)
            .Where(log => log != null)
            .Select(log => log!)
            .OrderByDescending(name => name)
            .ToList();

        foreach (var log in logFiles)
        {
            Logs.Add(log);
        }
    }

    private void OpenLog()
    {
        if (SelectedLog == null)
        {
            MessageBox.Show(easySave.GetText("box.logs"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string logsPath = _logger.GetLogFormatter() is JsonLogFormatter
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "JSON")
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "XML");

        string filePath = Path.Combine(logsPath, SelectedLog + (_logger.GetLogFormatter() is JsonLogFormatter ? ".json" : ".xml"));

        if (!File.Exists(filePath))
        {
            MessageBox.Show(easySave.GetText("box.logs_exist"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", filePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", filePath);
            }
            else
            {
                MessageBox.Show(easySave.GetText("box.os"), easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{easySave.GetText("box.logs_error")} : {ex.Message}", easySave.GetText("box.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ToggleLogFormat()
    {
        bool isCurrentlyJson = _logger.GetLogFormatter() is JsonLogFormatter;

        _logger.SetLogFormatter(isCurrentlyJson ? new XmlLogFormatter() : new JsonLogFormatter());

        string newFormat = isCurrentlyJson ? "XML" : "JSON";
        MessageBox.Show($"{easySave.GetText("box.logs_format")} {newFormat} !", easySave.GetText("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);


        LoadLogs();
    }

    private Language currentLanguage = new EnLanguage();
    public string this[string key] => easySave.GetText(key);

    private void ToggleLanguage()
    {
        currentLanguage = currentLanguage is FrLanguage ? new EnLanguage() : new FrLanguage();
        easySave.SetLanguage(currentLanguage);
        UpdateLanguage();
    }

    private void UpdateLanguage()
    {
        OnPropertyChanged("");
    }


    private void OpenSettings()
    {
        var settingsWindow = new SettingsWindow
        {
            Owner = Application.Current.MainWindow // Définit la fenêtre principale comme propriétaire
        };
        settingsWindow.ShowDialog();
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
            MessageBox.Show("Veuillez sélectionner un fichier .exe valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}

    private void AddBusinessApplication()
    {
        if (!string.IsNullOrWhiteSpace(BusinessApplicationsPath))
        {
            // Vérifier si le chemin pointe bien vers un fichier .exe
            if (System.IO.Path.GetExtension(BusinessApplicationsPath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                if (!BusinessApplications.Contains(BusinessApplicationsPath))
                {
                    BusinessApplications.Add(BusinessApplicationsPath);
                    MessageBox.Show("Application ajoutée avec succès !", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("application dèja ajoutée", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un fichier .exe valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        else
        {
            MessageBox.Show("Veuillez d'abord sélectionner un chemin d'application.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }



    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
