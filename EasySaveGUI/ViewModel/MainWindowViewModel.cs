using Avalonia.Threading;
using Avalonia.ReactiveUI; // Assurez-vous que ReactiveUI pour Avalonia est installé
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace EasySaveGUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        // Liste des sauvegardes
        public ObservableCollection<string> Backups { get; } = new ObservableCollection<string>();

        // Commandes pour les boutons
        public ReactiveCommand<Unit, Unit> AddBackupCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteBackupCommand { get; }
        public ReactiveCommand<Unit, Unit> RunBackupCommand { get; }
        public ReactiveCommand<Unit, Unit> RestoreBackupCommand { get; }
        public ReactiveCommand<Unit, Unit> ViewLogsCommand { get; }
        public ReactiveCommand<Unit, Unit> ViewStateCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleLanguageCommand { get; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }

        // Texte de la barre de statut
        private string _statusText = "Statut : Prêt";
        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }

        private bool isFrench = true;

        public MainWindowViewModel()
        {
            // Initialisation des commandes avec exécution sur le thread UI
            AddBackupCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    string backupName = $"Sauvegarde {Backups.Count + 1}";
                    Backups.Add(backupName);
                    StatusText = $"Ajouté : {backupName}";
                });
            });

            DeleteBackupCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (Backups.Count > 0)
                    {
                        string removedBackup = Backups[^1];
                        Backups.RemoveAt(Backups.Count - 1);
                        StatusText = $"Supprimé : {removedBackup}";
                    }
                    else
                    {
                        StatusText = "Aucune sauvegarde à supprimer.";
                    }
                });
            });

            RunBackupCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    StatusText = "Exécution d'une sauvegarde...";
                });
            });

            RestoreBackupCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    StatusText = "Restauration d'une sauvegarde...";
                });
            });

            ViewLogsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    StatusText = "Affichage des logs...";
                });
            });

            ViewStateCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    StatusText = "Affichage de l'état...";
                });
            });

            ToggleLanguageCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    isFrench = !isFrench;
                    StatusText = isFrench ? "Langue : Français" : "Language: English";
                });
            });

            ExitCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Environment.Exit(0);
                });
            });
        }
    }
}
