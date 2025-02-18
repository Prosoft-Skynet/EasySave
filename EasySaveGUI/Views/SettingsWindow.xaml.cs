namespace EasySaveGUI.Views;

using System.Windows;
using EasySaveGUI.ViewModels;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        if (Application.Current.MainWindow?.DataContext is MainViewModel mainViewModel)
        {
            DataContext = mainViewModel;
        }
        else
        {
            MessageBox.Show("Le DataContext n'a pas pu être initialisé.");
        }
    }
}
