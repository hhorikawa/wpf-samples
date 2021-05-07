using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace SimpleEditor.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();

        var x = (CollectionViewSource) FindResource("layersVS1");
#if WIP
        x.SortDescriptions.Clear();
        // TODO: これでは正しくない。インデックスで並べ替えないといけない.
        x.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Descending));
#endif
    }
} // class MainView

}
