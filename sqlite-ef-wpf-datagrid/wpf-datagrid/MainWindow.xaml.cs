using System;
using System.Collections.Generic; // Dictionary
using System.Collections.ObjectModel; // ObservableCollection
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input; // ICommand
using System.Linq; // error CS1936: ソース型 'DbSet<SalesOrder>' のクエリ パターンの実装が見つかりませんでした。'Select' が見つかりません。

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
  候補は次の3つ;
    - System.Windows.Controls.ItemCollection
            > WPF 各コントロールの内容を保持する。目的が違う。
    - System.Windows.Data.BindingListCollectionView: CollectionView から派生
            > データのコンテナは IBindingList を実装.
    - System.Windows.Data.ListCollectionView: CollectionView から派生
            > データのコンテナは IList を実装.
  CollectionView.SortDescriptions property で並べ替えを指定。
 */
class MainViewModel : DependencyObject
{
    // DataGrid.ItemsSource
    public static readonly DependencyProperty GridItemsProperty =
            DependencyProperty.Register("GridItems",
                                    typeof(System.Collections.IList),
                                    typeof(MainViewModel));

    // コンストラクタ
    public MainViewModel()
    {
        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.OrderDetail,
                                       OrderDetailExecuted, CanOrderDetail));

        MyCommands.CommandBindings.Add(
                new CommandBinding(MyCommands.ListOrders, ListOrdersExecuted));
    }

    // 注文の詳細...
    void OrderDetailExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    void CanOrderDetail(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    void ListOrdersExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Model1 model = new Model1();
        var query = (from t in model.SalesOrders
                    orderby t.Id
                    select t).Take(500);
        var list = query.ToList();
        SetValue(GridItemsProperty, list);
    }
}


[ValueConversion(typeof(string), typeof(Uri))] // sourceType, targetType
public class EmailConverter : IValueConverter
{
    // The source (viewmodel) to the target (WPF element)
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Uri email = new Uri("mailto:" + (string) value);
        return email;
    }

    // The target (WPF element) to the source (viewmodel)
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var dc = DataContext as MainViewModel;

        // CommandBindings は readonly のため、単に代入は不可
        foreach (var binding in MyCommands.CommandBindings)
            CommandBindings.Add(binding);
    }
} // class MainWindow

}