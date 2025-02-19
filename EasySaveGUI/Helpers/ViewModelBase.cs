using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySaveGUI.Helpers;

/// <summary>
/// Base class for view models.
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = delegate { }; // ✅ Initialise pour éviter null

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
