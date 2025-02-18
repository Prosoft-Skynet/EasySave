namespace EasySaveGUI.Views;

using System.Windows;
using EasySaveCore.src.Services;
using EasySaveGUI.ViewModels;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();

    }
}
