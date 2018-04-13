using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FanOutClassLibrary
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string name = "")
        {
            if (object.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            NotifyPropertyChanged(name);
            return true;
        }

        public void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
