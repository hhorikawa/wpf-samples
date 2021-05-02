using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; // CommandBinding
using wpf_datagrid.Models;

namespace wpf_datagrid
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
public partial class App : Application
{
    public readonly Model1 dbContext;

    App()
    {
        // DBに接続
        dbContext = new Model1();
    }


    void Application_Startup(object sender, StartupEventArgs e)
    {
        // コマンドと実行する関数とを紐付ける. "コマンド" として標準コマンドも
        // 使える。
        // コマンドは、それ自身を識別するのみ。実行されるコードは含まない。わ
        // かりにくい。
        MyCommands.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, FileExitCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.NewSalesOrder,
                                   NewSalesOrderCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.NewCustomer, NewCustomerCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.Window_SalesOrderList,
                                   SalesOrderListCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.Window_CustomerList,
                                   CustomerListCommand));

        var w = new MainWindow();
        w.Show();
    }


    // 新しい顧客...
    void NewCustomerCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new CustomerEditWindow(0);
        dialog.Show();
    }

    // Menu/ウィンドウ -> SalesOrder List
    void SalesOrderListCommand(object sender, ExecutedRoutedEventArgs e)
    {
        // 気にせずどんどん開く
        var w = new MainWindow();
        w.Show();
    }

    // Menu/ウィンドウ -> Customer List
    void CustomerListCommand(object sender, ExecutedRoutedEventArgs e)
    {
        // 気にせずどんどん開く
        var w = new CustomerListWindow();
        w.Show();
    }


    void FileExitCommand( object sender, ExecutedRoutedEventArgs e )
    {
        // ここで、保存しますか? などを出す
        Application.Current.Shutdown();
    }

    // [SalesOrder 一覧] ウィンドウ -> 新しい受注...
    void NewSalesOrderCommand(object sender, ExecutedRoutedEventArgs e)
    {
        // 気にせずどんどん開く。
        var dialog = new SalesOrderEditWindow(0);
        dialog.Show();
    }

} // class App

}
