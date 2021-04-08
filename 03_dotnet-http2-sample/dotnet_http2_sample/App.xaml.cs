
using System.Windows;
using System.Windows.Input; // CommandBindingCollection

namespace dotnet_http2_sample
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
                new CommandBinding(ApplicationCommands.Close, FileExit));

        var window = new MainWindow();
        window.Show();
    }

    void FileExit( object sender, ExecutedRoutedEventArgs e )
    {
        Application.Current.Shutdown();
    }

} // class App

}
