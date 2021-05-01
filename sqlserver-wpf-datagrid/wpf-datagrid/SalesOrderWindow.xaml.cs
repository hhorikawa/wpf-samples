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

namespace wpf_datagrid
{
    /// <summary>
    /// SalesOrderWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SalesOrderWindow : Window
    {
        public SalesOrderWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource salesOrderViewSource = ((System.Windows.Data.CollectionViewSource) (this.FindResource("salesOrderViewSource")));
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
            // salesOrderViewSource.Source = [汎用データ ソース]
        }
    }
}
