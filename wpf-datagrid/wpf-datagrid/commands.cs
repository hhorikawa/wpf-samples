using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // CommandBinding

namespace wpf_datagrid
{

static class MyCommands
{
    // CommandBindingCollection クラスは非ジェネリックな IList から派生.
    public static readonly List<CommandBinding> CommandBindings =
                                             new List<CommandBinding>();

    // コマンド -- Static にするのがキモ!

    // MainWindow -> [新しい注文...] button
    static readonly RoutedUICommand _newOrder = 
            new RoutedUICommand("新しい注文", "NewOrder", typeof(MyCommands));
    public static RoutedUICommand NewOrder { 
        get { return _newOrder; }
    }

    // MainWindow -> [詳細] button
    static readonly RoutedUICommand _orderDetail = 
            new RoutedUICommand("注文の詳細を確認", "OrderDetail", typeof(MyCommands));
    public static RoutedUICommand OrderDetail { 
        get { return _orderDetail; }
    }

} // class MyCommands

}
