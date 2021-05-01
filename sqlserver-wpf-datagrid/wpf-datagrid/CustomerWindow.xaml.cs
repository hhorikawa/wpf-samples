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
public partial class CustomerWindow : Window
{
    public CustomerWindow()
    {
        InitializeComponent();
    }
/*
    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        System.Windows.Data.CollectionViewSource customerViewSource = 
                ((System.Windows.Data.CollectionViewSource) (this.FindResource("customerViewSource")));
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
            // customerViewSource.Source = [汎用データ ソース]
    }
*/
    private void okButton_Click(object sender, RoutedEventArgs ev)
    {
        Customer r = (Customer) DataContext;
        var dbContext = ((App) App.Current).dbContext;
        dbContext.Customers.Add(r);
        try { 
            dbContext.SaveChanges();
        }
        catch (DbEntityValidationException ex) {
            Console.WriteLine(ex.ToString());
        }
    }
} // class CustomerWindow

}
