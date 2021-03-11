using System;
using System.Collections.Generic; // Dictionary
using System.Collections.ObjectModel; // ObservableCollection
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input; // ICommand

namespace wpf_datagrid
{


public class Order
{
    public enum OrderStatus {
        New = 0,
        Processing = 1,
        Shipped = 2,
        Paid = 3,
    };

    public static readonly Dictionary<OrderStatus, string> StatusList = new Dictionary<OrderStatus, string>() {
        {OrderStatus.New,        "New"},
        {OrderStatus.Processing, "Processing" },
        {OrderStatus.Shipped, "Shipped"},
        {OrderStatus.Paid, "Paid"},
    };

    public string OrderNumber { get; set; }

    ///////////////////////
    // Customer
    public string ShipTo { get; set; }

    public string Email { get; set; }

    // 0 = 通常, 1 = Gold
    public int Grade { get; set; }

    ///////////////////////

    public OrderStatus Status { get; set; }
}


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
    public static readonly DependencyProperty GridItemsProperty =
            DependencyProperty.Register("GridItems",
                                    typeof(ListCollectionView),
                                    typeof(MainViewModel));

    // コンストラクタ
    public MainViewModel()
    {
        MyCommands.CommandBindings.Add(
                    new CommandBinding(MyCommands.OrderDetail,
                                       OrderDetailExecuted, CanOrderDetail));
    }

    void OrderDetailExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    void CanOrderDetail(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
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

        // TODO: 適当に1,000件作る
        var dataTable = new ObservableCollection<Order>() {
                new Order() {OrderNumber = "a1", ShipTo = "2田中太郎",
                    Email = "3abc@example.com", Grade = 0,
                    Status = Order.OrderStatus.Processing},
                new Order() {OrderNumber = "a2", ShipTo = "3田中太郎",
                    Email = "4abc@example.com", Grade = 0,
                    Status = Order.OrderStatus.Processing},
                new Order() {OrderNumber = "a3", ShipTo = "4田中太郎",
                    Email = "1abc@example.com", Grade = 0,
                    Status = Order.OrderStatus.Processing},
                new Order() {OrderNumber = "a4", ShipTo = "1田中太郎",
                    Email = "2abc@example.com", Grade = 0,
                    Status = Order.OrderStatus.Processing},
            };
        dc.SetValue(MainViewModel.GridItemsProperty, new ListCollectionView(dataTable));
    }

} // class MainWindow

}
