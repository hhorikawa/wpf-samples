
namespace DataVirtualization
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
public partial class App
{
    void Application_Startup(object sender, System.Windows.StartupEventArgs e)
    {
        var window = new DemoWindow();
        window.Show();
    }
}

}
