using System;
using System.Collections.Generic;
using System.Globalization; // CultureInfo
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpf_datagrid.Models;

namespace wpf_datagrid
{

/*
  DataGrid の ItemsSource には, INotifyCollectionChanged interface を実装する
  か, ObservableCollection<T> class かその派生クラスを bind する。
  CLR List<T> class は推奨されない.
  条件: 非ジェネリックな IList から派生していること。ObservableCollection<T> は
        条件を満たす.
  => この場合, 内部でデフォルト collection view が使われる

  ソートさせたいときは, .NET 4.0 までと .NET 4.5 以降で異なる。
  [.NET 4.0まで]
  CollectionViewSource class (の View プロパティ?) を ItemsSource に bind す
  る。
  その Source プロパティにデータコレクションを set する。
  <CollectionViewSource.SortDescriptions> で並べ替えフィールドを指定

  [.NET 4.5以降]
  ICollectionViewLiveShaping インタフェイスを実装する collection view クラスで
  データコレクションを wrapし、collection view を ItemsSource に bind する。
  候補は次の2つ;
    - System.Windows.Data.BindingListCollectionView: CollectionView から派生
            > データのコンテナは IBindingList を実装.
    - System.Windows.Data.ListCollectionView: CollectionView から派生
            > データのコンテナは IList を実装.
  CollectionView.SortDescriptions property で並べ替えを指定。
  `System.Windows.Controls.ItemCollection` は WPF 各コントロールの内容を保持する。目的が違う。
 */

class MainViewModel : DependencyObject
{
    // DataGrid.ItemsSource -> soViewSource
    // CollectionViewSource.Source -> GridItems
    public static readonly DependencyProperty GridItemsProperty =
            DependencyProperty.Register("GridItems",
                                typeof(ExObservableCollection<SalesOrder>),
                                typeof(MainViewModel));

    static MainViewModel()
    {
        // 2回 Add() しても、最初のほうが優先される。
        // なので、コンストラクタで Add() しても意味ない.
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.FilterOrders, FilterOrdersExecuted));
    }


    // コンストラクタ
    public MainViewModel()
    {
        var app = (MyApp) Application.Current;
        app.SalesOrderUpdated += OnSalesOrderUpdated;

        SetValue(GridItemsProperty, new ExObservableCollection<SalesOrder>());
        itemsRefresh();
    }

    void itemsRefresh()
    {
        var items = (ExObservableCollection<SalesOrder>) GetValue(GridItemsProperty);
        items.Clear();
        // TODO: フィルタの考慮
        var query = (from so in MyApp.dbContext.SalesOrders
                     join c in MyApp.dbContext.Customers on so.CustomerId equals c.Id
                     orderby so.Id
                     select so).Take(500).ToList<SalesOrder>();
        items.Clear();
        items.AddRange(query);
    }

    private void OnSalesOrderUpdated(object sender, EventArgs e)
    {
        itemsRefresh();
    }

    // ウィンドウを複数開くときは, static メソッドにしたうえで, sender に基づ
    // いて、どのウィンドウで呼び出されたか判定.
    static void FilterOrdersExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        MainWindow view = (MainWindow) sender;
        MainViewModel self = (MainViewModel) view.DataContext;
        self.itemsRefresh();
    }
}


// View model を使う例 
public partial class MainWindow : Window
{
    // コンストラクタ
    public MainWindow()
    {
        InitializeComponent();

        // CommandBindings は readonly のため、単に代入は不可
        foreach (var binding in MyCommands.CommandBindings)
            CommandBindings.Add(binding);
    }

} // class MainWindow


}
