
using System.Windows;

namespace UF02UwpDesktop
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
    }
}

}
