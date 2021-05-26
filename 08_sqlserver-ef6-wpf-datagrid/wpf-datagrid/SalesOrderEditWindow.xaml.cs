using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
/*
// Entity Framework レコードクラスから派生させるときは, [NotMapped] を付けない
// と, DB に保存しない場合であっても, "Discriminator" 列が自動的に生成される.
// => そもそも, 派生クラスは, dbContext に追加する時点でエラーになる。
//    DB に保存しない class に派生させるのは上手くいかない.
[NotMapped]
class ExSalesOrder : SalesOrder, INotifyPropertyChanged
{
}
*/

    /// <summary>
    /// SalesOrderWindow.xaml の相互作用ロジック
    /// </summary>
public partial class SalesOrderEditWindow : Window
{
    event EventHandler Changed;

    public SalesOrderEditWindow(int id, EventHandler changedHandler)
    {
        InitializeComponent();

        Changed += changedHandler;

        if (id > 0) {
            var so = MyApp.dbContext.SalesOrders
                        .Where(s => s.Id == id)
                        .Include(s => s.Details) // Eager loading: 明細表に表示する
                        .First();
            DataContext = so;
            customerName.Text = so.Customer.FullName;
            okButton.Content = "Update";
        }
    }


    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // XAML 内のリソース要素を取得する。
        var salesOrderDetailViewSource =
            (CollectionViewSource) this.FindResource("salesOrderDetailsViewSource");
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
            // salesOrderViewSource.Source = [汎用データ ソース]
    }

    // [Create] / [Update]
    void okButton_Click(object sender, RoutedEventArgs e)
    {
        SalesOrder so = (SalesOrder) DataContext;

        so.LockVersion++; // TODO: check
        var customer = MyApp.dbContext.Customers.First(c => c.Id == so.CustomerId);
        if (so.CustomerShipTo != customer.ShipTo)
            customer.ShipTo = so.CustomerShipTo;

        if (so.Id == 0)
            MyApp.dbContext.SalesOrders.Add(so);

        try {
            MyApp.dbContext.SaveChanges();
        }
        catch (DbEntityValidationException ex) {
            var msg = "";
            foreach (var err in ex.EntityValidationErrors) {
                foreach (var f in err.ValidationErrors) {
                    msg += f.ErrorMessage;
                }
            }
            errMsg.Text = msg;
            errMsg.Visibility = Visibility.Visible;
            return;
        }

        Changed.Invoke(this, null);
        Close();
    }

    void cancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }


    private void customerPickUpButton_Click(object sender, RoutedEventArgs e)
    {
        var so = (SalesOrder) DataContext;

        var dialog = new CustomerPickUp();
        if (dialog.ShowDialog() == true) {
            var c = MyApp.dbContext.Customers.Single(p => p.Id == dialog.customerId);

            so.CustomerId = c.Id;
            // 値のコピー。記録のため、変化しうるものはコピーすべき (定石)
            so.CustomerShipTo = c.ShipTo;
            //so.RaisePropertyChanged("CustomerShipTo");

            // 表示だけ更新
            customerName.Text = c.FullName;
        }
    }


    // "新しい明細" グループボックス -> [製品の選択] ボタン
    private void productPickUpButton_Click(object sender, RoutedEventArgs e)
    {
        var sod = (SalesOrderDetail) newDetail.DataContext;

        var dialog = new ProductPickUp();
        if (dialog.ShowDialog() == true) {
            var pro = MyApp.dbContext.Products
                                    .Single(x => x.Id == dialog.productId);
            sod.ProductId = pro.Id;
            sod.Product = pro;
            productName.Text = pro.Name;
        }
    }

    // "新しい明細" グループボックス -> [追加] ボタン
    private void detailAddButton_Click(object sender, RoutedEventArgs e)
    {
        var so = (SalesOrder) DataContext;
        var sod = (SalesOrderDetail) newDetail.DataContext;

        sod.Status = SalesOrderStatus.New;
        if (sod.ProductId == 0) {
            MessageBox.Show("製品が選択されていません");
            return;
        }
        if (sod.Comment == null)
            sod.Comment = "";

        so.Details.Add(sod);
        newDetail.DataContext = new SalesOrderDetail();
        salesOrderDetailsDataGrid.Items.Refresh();
    }
} // class SalesOrderWindow

}
