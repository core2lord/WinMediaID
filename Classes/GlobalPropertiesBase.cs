using System.Collections.Generic;
using System.ComponentModel;

namespace WinMediaID;

public class GlobalPropertiesBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnNotifyPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool SetProperty<T>(ref T field, T value, string propertyName)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnNotifyPropertyChanged(propertyName);
        return true;
    }
}