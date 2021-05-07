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
public partial class MyApp : Application
{
    public readonly Model1 dbContext;

    public List<CustomerListWindow> customerListWindows =
                                    new List<CustomerListWindow>();

    public Dictionary<int, CustomerEditWindow> customerEditWindows =
                                    new Dictionary<int, CustomerEditWindow>();
    public Dictionary<int, SalesOrderEditWindow> soEditWindows =
                                    new Dictionary<int, SalesOrderEditWindow>();

    // コンストラクタ
    MyApp()
    {
        // DBに接続
        dbContext = new Model1();
    }


    /// //////////////////////////////////////////////////////////////
    /// Event handlers

    void Application_Startup(object sender, StartupEventArgs e)
    {
        // コマンドと実行する関数とを紐付ける. "コマンド" として標準コマンドも
        // 使える。
        // コマンドは、それ自身を識別するのみ。実行されるコードは含まない。わ
        // かりにくい。

        // Menu
        MyCommands.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, FileExitCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.Window_SalesOrderList,
                                   SalesOrderListCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.Window_CustomerList,
                                   CustomerListCommand));

        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.NewSalesOrder,
                                   NewSalesOrderCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.NewCustomer, NewCustomerCommand));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.SalesOrderDetail,
                                   SalesOrderDetailExecuted, CanSalesOrderDetail));
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.CustomerDetail,
                                   CustomerDetailExecuted, CanCustomerDetail));

        var w = new MainWindow();
        w.Show();
    }


    void OnCustomerChanged(object sender, EventArgs e)
    {
        foreach (var w in customerListWindows) {
            w.CustomerUpdated();
        }
    }


    /// //////////////////////////////////////////////////////////////
    /// Command handlers

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
        customerListWindows.Add(w);
        w.Show();
    }

    // Menu/ファイル -> 終了
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

    // 新しい顧客...
    void NewCustomerCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new CustomerEditWindow(0, OnCustomerChanged);
        dialog.Show();
    }

    // [SalesOrder 一覧] ウィンドウ -> 受注の詳細...
    void SalesOrderDetailExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        // ただ一つの編集ウィンドウを表示する、挙動のほうが簡単。
        // ここでは, ウィンドウをリサイクルする例.

        throw new NotImplementedException();
    }

    // コマンドが実行可能かどうか
    void CanSalesOrderDetail(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    // Command を使う場合, e.Parameter で必要なデータをやり取りする。
    // XAML 側で CommandParameter を指定しない場合, null.
    void CustomerDetailExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        int id = ((Customer) e.Parameter).Id;
        if (customerEditWindows.ContainsKey(id))
            customerEditWindows[id].Focus();
        else {
            var dialog = new CustomerEditWindow(id, OnCustomerChanged);
            customerEditWindows.Add(id, dialog);
            dialog.Show();
        }
    }

    void CanCustomerDetail(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

} // class App

}
