using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM
{

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    //basic ViewModelBase
    internal void RaisePropertyChanged([CallerMemberName] string prop = null)
    {
        if (PropertyChanged != null) {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

} // class ViewModelBase

}
