using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;  // IValueConverter
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wpf_datagrid.Models;

namespace wpf_datagrid
{

// メソッドの直行性が全然ない。
// ObservableCollection<> に AddRangeメソッドを追加する。
// ちなみに, 要素一つずつ Add() すると, つどイベントが発火するのでパフォーマン
// スが酷い。
// ちゃんとした実装は、例えばこちら;
//    http://artfulplace.hatenablog.com/entry/2016/12/29/133950
public class ExObservableCollection<T>:
                    System.Collections.ObjectModel.ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> collection)
    {
        ((List<T>) this.Items).AddRange(collection);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}


// MainWindow と異なり, view model を使わない例
public partial class CustomerListWindow : Window
{
    readonly ExObservableCollection<Customer> _customerList =
                                    new ExObservableCollection<Customer>();

    public CustomerListWindow()
    {
        InitializeComponent();

        // CommandBindings は readonly のため、単に代入は不可
        foreach (var binding in MyCommands.CommandBindings)
            CommandBindings.Add(binding);
    }

    // Event handler
    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _customerList.AddRange(from c in MyApp.dbContext.Customers
                               select c);

        // XAML Window.Resources の要素を取得
        var customerViewSource =
                (CollectionViewSource) this.FindResource("customerViewSource");
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
        customerViewSource.Source = _customerList;
        // 何も指定しない場合は, CollectionViewSource.View は,
        // System.Windows.Data.ListCollectionView になる。
    }


    internal void OnCustomerUpdated()
    {
        _customerList.Clear();
        _customerList.AddRange(from c in MyApp.dbContext.Customers
                               select c);
    }

} // class CustomerListWindow 

}
