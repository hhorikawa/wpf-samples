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
    public static Model1 dbContext;

    List<CustomerListWindow> customerListWindows =
                                    new List<CustomerListWindow>();
    List<MainWindow> soListWindows = new List<MainWindow>();

    Dictionary<int, CustomerEditWindow> customerEditWindows =
                                    new Dictionary<int, CustomerEditWindow>();
    Dictionary<int, SalesOrderEditWindow> soEditWindows =
                                    new Dictionary<int, SalesOrderEditWindow>();

    public event EventHandler SalesOrderUpdated;

    // コンストラクタ
    MyApp()
    {
        if (dbContext == null) { 
            // DBに接続
            dbContext = new Model1();
        }
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


    // 直接 list window で受けようとすると, 多:多になってしまう。
    void OnCustomerChanged(object sender, EventArgs e)
    {
        foreach (var w in customerListWindows) {
            w.OnCustomerUpdated();
        }
    }

    void OnSalesOrderChanged(object sender, EventArgs e)
    {
        // 単に発火させる
        SalesOrderUpdated(sender, e);
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
    void CustomerListCommand(object sender, ExecutedRoutedEventArgs args)
    {
        // 気にせずどんどん開く
        var w = new CustomerListWindow();
        customerListWindows.Add(w);
        w.Closed += (s, e) => { customerListWindows.Remove(w); };
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
        var dialog = new SalesOrderEditWindow(0, OnSalesOrderChanged);
        dialog.Show();
    }

    // 新しい顧客...
    void NewCustomerCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new CustomerEditWindow(0, OnCustomerChanged);
        dialog.Show();
    }

    // [SalesOrder 一覧] ウィンドウ -> 受注の詳細...
    void SalesOrderDetailExecuted(object sender, ExecutedRoutedEventArgs args)
    {
        // ただ一つの編集ウィンドウを表示する、という挙動のほうが簡単。
        // ここでは, ウィンドウをリサイクルする例.
        int id = ((SalesOrder) args.Parameter).Id;
        if (soEditWindows.ContainsKey(id))
            soEditWindows[id].Focus();
        else {
            var dialog = new SalesOrderEditWindow(id, OnSalesOrderChanged);
            soEditWindows.Add(id, dialog);
            dialog.Closed += (s, e) => { soEditWindows.Remove(id); };
            dialog.Show();
        }
    }

    // コマンドが実行可能かどうか
    void CanSalesOrderDetail(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    // Command を使う場合, e.Parameter で必要なデータをやり取りする。
    // XAML 側で CommandParameter を指定しない場合, null.
    void CustomerDetailExecuted(object sender, ExecutedRoutedEventArgs args)
    {
        int id = ((Customer) args.Parameter).Id;
        if (customerEditWindows.ContainsKey(id))
            customerEditWindows[id].Focus();
        else {
            var dialog = new CustomerEditWindow(id, OnCustomerChanged);
            customerEditWindows.Add(id, dialog);
            dialog.Closed += (s, e) => { customerEditWindows.Remove(id); };
            dialog.Show();
        }
    }

    void CanCustomerDetail(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

} // class App

}
