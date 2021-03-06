using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; // CommandBinding 

namespace wpf_datagrid
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
public partial class App : Application
{
    void Application_Startup(object sender, StartupEventArgs e)
    {
        // コマンドと実行する関数とを紐付ける.
        // "コマンド" として標準コマンドも使える。
        MyCommands.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, FileExitCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.NewOrder, NewOrderCommand));

        var window1 = new MainWindow();
        window1.Show();

        var window2 = new MainWindow();
        window2.Show();
    }


    void FileExitCommand( object sender, ExecutedRoutedEventArgs e )
    {
        Application.Current.Shutdown();
    }

    void NewOrderCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new OrderEditWindow();
        dialog.Show();
    }

} // class App

}
