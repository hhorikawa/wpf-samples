
using System.Windows;

namespace TestDataGrid
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
public partial class App : Application
{
    void Application_Startup(object sender, StartupEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
    }
} // class App

}
