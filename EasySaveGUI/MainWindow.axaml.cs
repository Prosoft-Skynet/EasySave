using Avalonia.Controls;
using EasySaveGUI.ViewModels;

namespace EasySaveGUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(); // Liaison du ViewModel
        }
    }
}