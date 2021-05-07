using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // CommandBinding

namespace wpf_datagrid
{

// コマンドを並べる
static class MyCommands
{
    // CommandBindingCollection クラスは非ジェネリックな IList から派生.
    public static readonly List<CommandBinding> CommandBindings =
                                             new List<CommandBinding>();

    // コマンド -- Static にするのがキモ!

    // MainWindow -> [新しい注文...] button
    static readonly RoutedUICommand _newOrder = 
            new RoutedUICommand("新しい注文", nameof(NewSalesOrder), 
                                typeof(MyCommands));
    public static RoutedUICommand NewSalesOrder { 
        get { return _newOrder; }
    }

    static readonly RoutedUICommand _newCustomer = 
            new RoutedUICommand("新しい顧客", nameof(NewCustomer), 
                                typeof(MyCommands));
    public static RoutedUICommand NewCustomer { 
        get { return _newCustomer; }
    }

    // SalesOrder List Window -> [フィルタ] button
    static readonly RoutedUICommand _listOrders = 
            new RoutedUICommand("受注の一覧", nameof(FilterOrders), 
                                typeof(MyCommands));
    public static RoutedUICommand FilterOrders { 
        get { return _listOrders; }
    }

    // MainWindow -> [詳細] button
    static readonly RoutedUICommand _orderDetail = 
            new RoutedUICommand("受注の詳細を確認", nameof(SalesOrderDetail), 
                                typeof(MyCommands));
    public static RoutedUICommand SalesOrderDetail { 
        get { return _orderDetail; }
    }

    // Customer List Window -> [詳細] button
    static readonly RoutedUICommand _customerDetail = 
            new RoutedUICommand("顧客の詳細を確認", nameof(CustomerDetail), 
                                typeof(MyCommands));
    public static RoutedUICommand CustomerDetail { 
        get { return _customerDetail; }
    }

    static readonly RoutedUICommand _newWindow =
            new RoutedUICommand("SalesOrder一覧ウィンドウ",
                                nameof(Window_SalesOrderList), 
                                typeof(MyCommands));
    public static RoutedUICommand Window_SalesOrderList {
        get { return _newWindow; }
    }

    static readonly RoutedUICommand _newCustomerWindow =
            new RoutedUICommand("SalesOrder一覧ウィンドウ",
                                nameof(Window_CustomerList), 
                                typeof(MyCommands));
    public static RoutedUICommand Window_CustomerList {
        get { return _newCustomerWindow; }
    }

} // class MyCommands

}
