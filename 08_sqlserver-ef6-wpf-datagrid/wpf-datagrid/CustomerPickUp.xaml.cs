using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using wpf_datagrid.Models;

namespace wpf_datagrid
{
    /// <summary>
    /// CustomerPickUp.xaml の相互作用ロジック
    /// </summary>
public partial class CustomerPickUp : Window
{
    public int customerId;

    readonly ExObservableCollection<Customer> _customerList =
                                    new ExObservableCollection<Customer>();

    public CustomerPickUp()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _customerList.AddRange(from c in MyApp.dbContext.Customers
                               select c);

        var customerViewSource =
                (CollectionViewSource) this.FindResource("customerViewSource");
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
        customerViewSource.Source = _customerList;
            // customerViewSource.Source = [汎用データ ソース]
    }

    private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var customer = (Customer) ((DataGridRow) sender).DataContext;
        customerId = customer.Id;
        this.DialogResult = true;
        Close();
    }

    private void pickButton_Click(object sender, RoutedEventArgs e)
    {
        var row = (Customer) customerDataGrid.SelectedItem;
        customerId = row.Id;
        this.DialogResult = true;
        Close();
    }
} // class CustomerPickUp

}
