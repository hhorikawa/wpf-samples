using System;
using System.Diagnostics;
using System.Windows.Input;

namespace MVVM
{
// T コマンドパラメータの型
public class RelayCommand<T> : ICommand
{
    // Fields ///////////////////////////////////////////

    readonly Action<T> _execute;
    readonly Predicate<T> _canExecute;

    // Constructors /////////////////////////////////////

    public RelayCommand(Action<T> execute)
            : this(execute, null)
    {
    }

    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException(nameof(execute));

        _execute = execute;
        _canExecute = canExecute == null ? (o) => true : canExecute;
    }


    [DebuggerStepThrough]
    public bool CanExecute(T parameter)
    {
        return _canExecute(parameter);
    }

    // コマンドが実行可能か、の状態変更で発火
    public event EventHandler CanExecuteChanged {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(T parameter)
    {
        _execute(parameter);
    }

    // 覆い隠す
    bool ICommand.CanExecute(object parameter)
    {
        return CanExecute((T) parameter);
    }

    void ICommand.Execute(object parameter)
    {
        Execute((T) parameter);
    }
} // class RelayCommand<T>

}
