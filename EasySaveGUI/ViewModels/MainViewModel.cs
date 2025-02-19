﻿namespace EasySaveGUI.ViewModels;

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

public class MainViewModel : ViewModelBase
{
    private LanguageService _languageService;
    private readonly BackupService _backubService;
    private BusinessApplicationService _businessApplicationService;
    private readonly Logger _logger;
    private string _backupName = string.Empty;
    private string _sourcePath = string.Empty;
    private string _destinationPath = string.Empty;
    private bool _isFullBackup = true;

    public ObservableCollection<BackupJobModel> Backups { get; }
    public ObservableCollection<string> Logs { get; }

    private BackupJobModel? _selectedBackup = new BackupJobModel();
    public BackupJobModel? SelectedBackup
    {
        get => _selectedBackup;
        set
        {
            _selectedBackup = value;

            if (_selectedBackup != null)
            {
                _backubService.SetBackupStrategy(_selectedBackup.IsFullBackup);
            }

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
    public ICommand ExitCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand PauseBackupCommand { get; }
    public ICommand ResumeBackupCommand { get; }
    public ICommand StopBackupCommand { get; }

    public string this[string key] => _languageService.GetTranslation(key);

    public MainViewModel()
    {
        _backubService = new BackupService();
        _languageService = new LanguageService();
        _businessApplicationService = new BusinessApplicationService();
        _logger = new Logger(new JsonLogFormatter());
        Backups = new ObservableCollection<BackupJobModel>(_backubService.GetBackupJobs());

        Backups = new ObservableCollection<BackupJobModel>(_backubService.LoadBackupJobs());
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
        ExitCommand = new RelayCommand(ExitApplication);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        PauseBackupCommand = new RelayCommand(PauseBackup);
        ResumeBackupCommand = new RelayCommand(ResumeBackup);
        StopBackupCommand = new RelayCommand(StopBackup);
    }

    private void AddBackup()
    {
        if (string.IsNullOrWhiteSpace(BackupName) || string.IsNullOrWhiteSpace(SourcePath) || string.IsNullOrWhiteSpace(DestinationPath))
        {
            MessageBox.Show(_languageService.GetTranslation("box.fill"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Backups.Any(b => b.Name == BackupName))
        {
            MessageBox.Show(_languageService.GetTranslation("box.name"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IBackupTypeStrategy strategy = IsFullBackup ? new CompleteBackupStrategy() : new DifferentialBackupStrategy();
            var job = new BackupJobModel(BackupName, SourcePath, DestinationPath, IsFullBackup);
            _backubService.AddBackup(job);
            _backubService.SaveBackupJobs();

            Backups.Add(job);
            MessageBox.Show($"{_languageService.GetTranslation("box.backup")} {BackupName} {_languageService.GetTranslation("box.create_success")}");

            BackupName = string.Empty;
            SourcePath = string.Empty;
            DestinationPath = string.Empty;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_languageService.GetTranslation("box.error")} : {ex.Message}", _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show(_languageService.GetTranslation("box.delete"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string backupName = SelectedBackup.Name;

        var jobToRemove = _backubService.GetBackupJobs().FirstOrDefault(b => b.Id == SelectedBackup.Id);
        if (jobToRemove != null)
        {
            _backubService.GetBackupJobs().Remove(jobToRemove);
        }

        Backups.Remove(SelectedBackup);
        _backubService.SaveBackupJobs();

        SelectedBackup = null;

        MessageBox.Show($"{_languageService.GetTranslation("box.backup")} {backupName} {_languageService.GetTranslation("box.delete_success")}");
    }

    private async void RunBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show(_languageService.GetTranslation("box.execute"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var startTime = DateTime.Now;

        Func<string, string> getEncryptionKeyCallback = (fileName) =>
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (!_backubService.extensionsToEncrypt.Contains(extension)) return string.Empty;

            string encryptionKey = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                do
                {
                    encryptionKey = CustomInputDialog.ShowDialog(
                        $"{_languageService.GetTranslation("menu.encrypt")} {fileName}",
                        _languageService.GetTranslation("menu.encryption_key"));
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
                _backubService.ExecuteJobinterface(SelectedBackup.Id, getEncryptionKeyCallback);
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

        MessageBox.Show($"{_languageService.GetTranslation("box.backup")} {SelectedBackup.Name} {_languageService.GetTranslation("box.execute_success")} {totalDurationMs} ms ! \n {_languageService.GetTranslation("menu.exception_application")} ", _languageService.GetTranslation("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);

        LoadLogs();
    }

    private async void RestoreBackup()
    {
        if (SelectedBackup == null)
        {
            MessageBox.Show(_languageService.GetTranslation("box.restore"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Func<string, string> getDecryptionKeyCallback = (fileName) =>
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (!_backubService.extensionsToEncrypt.Contains(extension)) return string.Empty;

            string decryptionKey = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                do
                {
                    decryptionKey = CustomInputDialog.ShowDialog(
                        $"{_languageService.GetTranslation("menu.decrypt")} {fileName}",
                        _languageService.GetTranslation("menu.decryption_key"));
                } while (string.IsNullOrEmpty(decryptionKey));
            });

            return decryptionKey;
        };

        try
        {
            await Task.Run(() =>
            {
                _backubService.RestoreJob(SelectedBackup.Id, getDecryptionKeyCallback);
            });

            MessageBox.Show($"{_languageService.GetTranslation("box.backup")} {SelectedBackup.Name} {_languageService.GetTranslation("box.restore_success")}",
                            _languageService.GetTranslation("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_languageService.GetTranslation("box.error")} : {ex.Message}",
                            _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SelectSource()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = _languageService.GetTranslation("box.files"),
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
            FileName = _languageService.GetTranslation("box.files"),
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
            MessageBox.Show(_languageService.GetTranslation("box.logs"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string logsPath = _logger.GetLogFormatter() is JsonLogFormatter
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "JSON")
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "XML");

        string filePath = Path.Combine(logsPath, SelectedLog + (_logger.GetLogFormatter() is JsonLogFormatter ? ".json" : ".xml"));

        executeLog(filePath);
    }

    private void executeLog(string filePath)
    {
        if (!File.Exists(filePath))
        {
            MessageBox.Show(_languageService.GetTranslation("box.logs_exist"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show(_languageService.GetTranslation("box.os"), _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_languageService.GetTranslation("box.logs_error")} : {ex.Message}", _languageService.GetTranslation("box.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ToggleLogFormat()
    {
        bool isCurrentlyJson = _logger.GetLogFormatter() is JsonLogFormatter;

        _logger.SetLogFormatter(isCurrentlyJson ? new XmlLogFormatter() : new JsonLogFormatter());

        string newFormat = isCurrentlyJson ? "XML" : "JSON";
        MessageBox.Show($"{_languageService.GetTranslation("box.logs_format")} {newFormat} !", _languageService.GetTranslation("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);

        LoadLogs();
    }

    private void ToggleLanguage()
    {
        _languageService.ChangeLanguage();
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
            Owner = Application.Current.MainWindow
        };
        settingsWindow.ShowDialog();
    }

    private void PauseBackup()
    {
        _backubService.PauseBackup();
        MessageBox.Show(_languageService.GetTranslation("box.pause_success"), _languageService.GetTranslation("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ResumeBackup()
    {
        _backubService.ResumeBackup();
        MessageBox.Show(_languageService.GetTranslation("box.resume_success"), _languageService.GetTranslation("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void StopBackup()
    {
        _backubService.StopBackup();
        MessageBox.Show(_languageService.GetTranslation("box.stop_success"), _languageService.GetTranslation("box.success"), MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
