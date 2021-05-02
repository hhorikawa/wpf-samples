using System;
using System.Collections.Generic;
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
    /// <summary>
    /// SalesOrderWindow.xaml の相互作用ロジック
    /// </summary>
public partial class SalesOrderEditWindow : Window
{
    readonly Model1 _dbContext = ((App) App.Current).dbContext;

    public SalesOrderEditWindow(int id)
    {
        InitializeComponent();

        if (id > 0) {
            var so = _dbContext.SalesOrders
                        .Where(s => s.Id == id)
                        .Include(s => s.Details) // 明細表に表示する
                        .First();
            DataContext = so;
            okButton.Content = "Update";
        }
    }

    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // XAML 内のリソース要素を取得する。
        var salesOrderDetailViewSource =
            (CollectionViewSource) (this.FindResource("salesOrderDetailsViewSource"));
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
            // salesOrderViewSource.Source = [汎用データ ソース]
    }

    void okButton_Click(object sender, RoutedEventArgs e)
    {
        SalesOrder so = (SalesOrder) DataContext;
        so.LockVersion++; // TODO: check
        if (so.Id == 0)
            _dbContext.SalesOrders.Add(so);
        try {
            _dbContext.SaveChanges();
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

        Close();
    }

    void cancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
} // class SalesOrderWindow

}
