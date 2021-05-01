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

    // MainWindow -> [一覧表示] button
    static readonly RoutedUICommand _listOrders = 
            new RoutedUICommand("受注の一覧", nameof(ListOrders), 
                                typeof(MyCommands));
    public static RoutedUICommand ListOrders { 
        get { return _listOrders; }
    }

    // MainWindow -> [詳細] button
    static readonly RoutedUICommand _orderDetail = 
            new RoutedUICommand("注文の詳細を確認", nameof(OrderDetail), 
                                typeof(MyCommands));
    public static RoutedUICommand OrderDetail { 
        get { return _orderDetail; }
    }

    static readonly RoutedUICommand _newWindow =
            new RoutedUICommand("新しいウィンドウ", nameof(NewWindow), 
                                typeof(MyCommands));
    public static RoutedUICommand NewWindow {
        get { return _newWindow; }
    }
} // class MyCommands

}
