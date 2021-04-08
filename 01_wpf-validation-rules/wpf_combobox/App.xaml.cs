
using System.Windows;

namespace wpf_combobox
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
public partial class App : Application
{
    // Application.Startup event:
    // Application オブジェクトの Run() メソッドが呼び出されたとき、発火.
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // 事前処理はここに書く.

        // 最初のウィンドウを作る
        MainWindow wnd = new MainWindow();
        wnd.Show();
    }
} // class App

}
