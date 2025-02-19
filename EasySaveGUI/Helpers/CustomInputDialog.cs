using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasySaveGUI.Helpers
{
    /// <summary>
    /// Can you update this summary 
    /// </summary>
    public class CustomInputDialog
    {
        public static string ShowDialog(string message, string title)
        {
            Window window = new Window
            {
                Title = title,
                Width = 350,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White
            };

            StackPanel stackPanel = new StackPanel { Margin = new Thickness(10) };
            TextBlock textBlock = new TextBlock { Text = message, Margin = new Thickness(0, 0, 0, 10) };
            TextBox textBox = new TextBox { Width = 300 };
            Button okButton = new Button { Content = "OK", Width = 80, Height = 25, Margin = new Thickness(0, 10, 0, 0) };

            okButton.Click += (sender, e) => window.DialogResult = true;

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(okButton);
            window.Content = stackPanel;

            if (window.ShowDialog() == true)
            {
                return textBox.Text.Trim();
            }

            return string.Empty;
        }
    }
}
