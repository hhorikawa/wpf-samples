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
    readonly Model1 _dbContext = ((App) App.Current).dbContext;

    public CustomerEditWindow(int id)
    {
        InitializeComponent();

        if (id > 0) {
            // 見つからないときは例外.
            var customer = _dbContext.Customers.Single(c => c.Id == id);
            DataContext = customer;
            okButton.Content = "Update";
        }
    }

    void okButton_Click(object sender, RoutedEventArgs ev)
    {
        Customer r = (Customer) DataContext;
        r.LockVersion++; // TODO: check
        if (r.Id == 0)
            _dbContext.Customers.Add(r);
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
} // class CustomerWindow

}
