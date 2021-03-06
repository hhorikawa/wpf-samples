using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace DataVirtualization
{
    /// <summary>
    /// Interaction logic for DemoWindow.xaml
    /// </summary>
public partial class DemoWindow
{
        /// <summary>
        /// Initializes a new instance of the <see cref="DemoWindow"/> class.
        /// </summary>
    public DemoWindow()
    {
        InitializeComponent();

        // use a timer to periodically update the memory usage
        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = new TimeSpan(0, 0, 1);
        timer.Tick += timer_Tick;
        timer.Start();
    }

    void timer_Tick(object sender, EventArgs e)
    {
        tbMemory.Text = string.Format("{0:0.00} MB", GC.GetTotalMemory(true)/1024.0/1024.0);
    }


    void refreshButton_Click(object sender, RoutedEventArgs e)
    {
        // create the demo items provider according to specified parameters
        int numItems = int.Parse(tbNumItems.Text);
        int fetchDelay = int.Parse(tbFetchDelay.Text);
        DemoCustomerProvider customerProvider = new DemoCustomerProvider(numItems, fetchDelay);

            // create the collection according to specified parameters
        int pageSize = int.Parse(tbPageSize.Text);
        int pageTimeout = int.Parse(tbPageTimeout.Text);

        // ListView.ItemsSource プロパティに設定
        if ( rbNormal.IsChecked.Value ) {
            // 同期する
            DataContext = new List<Customer>(
                    customerProvider.GetRange(0, customerProvider.Count().Result).Result);
        }
        else if ( rbVirtualizing.IsChecked.Value ) {
            DataContext = new VirtualizingCollection<Customer>(customerProvider, pageSize);
        }
        else if ( rbAsync.IsChecked.Value ) {
            DataContext = new AsyncVirtualizingCollection<Customer>(customerProvider, pageSize);
        }
    }
} // class DemoWindow

}
