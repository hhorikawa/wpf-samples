using System;
using System.Collections.Generic;
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

public class EnumStringConverter : IValueConverter
{
    // enum を文字列に変換する
    // enum 型は名前が取れる。すごいな。
    public object Convert(object value, Type targetType, object parameter,
                      CultureInfo culture)
    {
        if (value != null && value.GetType().IsEnum)
            return value.ToString();

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


    /// <summary>
    /// customer_list_window.xaml の相互作用ロジック
    /// </summary>
public partial class CustomerListWindow : Window
{
    readonly Model1 _dbContext = ((App) App.Current).dbContext;

    public CustomerListWindow()
    {
        InitializeComponent();

        // CommandBindings は readonly のため、単に代入は不可
        foreach (var binding in MyCommands.CommandBindings)
            CommandBindings.Add(binding);
    }

    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // XAML Window.Resources の要素を取得
        var customerViewSource = (CollectionViewSource) (this.FindResource("customerViewSource"));
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
        customerViewSource.Source = (from c in _dbContext.Customers
                                     select c).ToList();
    }

    void detailButton_Click(object sender, RoutedEventArgs e)
    {

    }

} // class CustomerListWindow 

}