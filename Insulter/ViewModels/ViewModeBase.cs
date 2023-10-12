using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Insulter.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler PropertyChanged;
    

    /// <summary>
    /// Updates property value if new value is different than exisiting and raises Changed event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="storage"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Object.Equals(storage, value))
        {
            return false;
        }
        storage = value;
        OnPropertyChanged(propertyName);

        return true;

    } //SetProperty


    /// <summary>
    /// Generates changed event for specified property
    /// </summary>
    /// <param name="propertyName">Name of changed property</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {

        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler is not null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    } //OnPropertyChanged

} //ViewModelBase

//Insulter.ViewModels
