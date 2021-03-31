
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TestDataGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    /// /////////////////////////////////////////////////////////////////
    // Event Handlers

    // Column header をクリックすると呼び出される.
    // e.Column.SortMemberPath -> "[2]" -- Binding Path の値
    void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        DataGrid dg = (DataGrid) sender;
        var cv = (DataGridCollectionView<DataRow>) dg.ItemsSource;
/*
        // e.Column.SortDirection は現在 (変更前) の値. null のことある。
        ListSortDirection direction = 
                e.Column.SortDirection == ListSortDirection.Ascending ? 
                    ListSortDirection.Descending : ListSortDirection.Ascending;
        cv.SortDescriptions.Add(
            new SortDescription(e.Column.SortMemberPath, direction));
 */
        cv.RefreshDataRows();
        cv.Refresh(); // Re-creates the view.

        // true で default sort を抑制.
        // => Default sort 内で sort direction などの更新も行われる。
        //    再描画はその後に呼び出されるので、抑制しなくてよい。
        //e.Handled = true; 
    }

} // class MainWindow

}
