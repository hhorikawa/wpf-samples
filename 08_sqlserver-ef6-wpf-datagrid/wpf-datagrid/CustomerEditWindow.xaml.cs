using System;
using System.Collections.Generic;
using System.Data.Entity.Validation; // DbEntityValidationException
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
    /// CustomerWindow.xaml の相互作用ロジック
    /// </summary>
public partial class CustomerEditWindow : Window
{
    event EventHandler Changed;

    public CustomerEditWindow(int id, EventHandler changedHandler)
    {
        InitializeComponent();

        Changed += changedHandler;

        if (id > 0) {
            // 見つからないときは例外.
            var customer = MyApp.dbContext.Customers.Single(c => c.Id == id);
            DataContext = customer;
            okButton.Content = "Update";
        }
    }

    void okButton_Click(object sender, RoutedEventArgs ev)
    {
        Customer r = (Customer) DataContext;

        // ここのサンプルコードでは, 自分でXAMLツリーを辿って,
        // 全部のコントロールが valid かどうか確認している。
        // https://docs.microsoft.com/ja-jp/dotnet/desktop/wpf/app-development/dialog-boxes-overview?view=netframeworkdesktop-4.8
        //   => BindingGroup を作ると、一撃で取れる。こっちのほうが良さそう。

        // XAML 検査器によるエラーメッセージ.
        // しかし、データベースも妥当性検査をするので、二度手間になる。
        // UIの観点でも、通ると思ったものが通らないと、適切ではない。
        // => 簡便なリアルタイム検査と caution のみにとどめ、
        //    エラーにするのはデータベースに任せるのがよい。
        grid1.BindingGroup.CommitEdit();
        if (Validation.GetHasError(grid1)) {
            string msg = "";
            foreach (ValidationError s in Validation.GetErrors(grid1)) {
                // .RuleInError は検査器を指す.
                msg += ((BindingExpression) s.BindingInError).ResolvedSourcePropertyName + ":" + s.ErrorContent;
            }
            errMsg.Text = msg;
            errMsg.Visibility = Visibility.Visible;
            return;
        }

        r.LockVersion++; // TODO: check
        if (r.Id == 0)
            MyApp.dbContext.Customers.Add(r);
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

} // class CustomerWindow

}
