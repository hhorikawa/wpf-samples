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
    /// ProductPickUp.xaml の相互作用ロジック
    /// </summary>
public partial class ProductPickUp : Window
{
    public int productId;

    readonly ExObservableCollection<Product> _productList =
                                    new ExObservableCollection<Product>();

    public ProductPickUp()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _productList.AddRange(from p in MyApp.dbContext.Products
                              select p);

        var productViewSource =
                (CollectionViewSource) this.FindResource("productViewSource");
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
            // productViewSource.Source = [汎用データ ソース]
        productViewSource.Source = _productList;
    }

    private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var prod = (Product) ((DataGridRow) sender).DataContext;
        productId = prod.Id;
        this.DialogResult = true;
        Close();
    }

    private void pickButton_Click(object sender, RoutedEventArgs e)
    {
        var row = (Product) productDataGrid.SelectedItem;
        productId = row.Id;
        this.DialogResult = true;
        Close();
    }
}

}
