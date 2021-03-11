
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TestDataGrid
{
    /// <summary>
    /// 
    /// </summary>    
class MyViewModel : DependencyObject // INotifyPropertyChanged
{
    const int COUNT = 10_000_000;  // C# 7.0 から "_" で見やすくできる

    #region Fields
    readonly DataTable dataTable;  // dummy
    Random rnd = new Random();
        #endregion

    #region Constructors
        /// <summary>
        /// Public constructor
        /// </summary>
    public MyViewModel()
    {
        dataTable = new DataTable();
        dataTable.Columns.Add("Dummy Column 1");
        dataTable.Columns.Add("Dummy Column 2");
        dataTable.Columns.Add("Dummy Column 3");

        var dataGridCollectionView = new DataGridCollectionView<DataRow>();
        dataGridCollectionView.ItemsCount += OnItemsCount;
        dataGridCollectionView.ItemsRequest += OnItemsRequest;

        SetValue(TableSourceProperty, dataGridCollectionView);
    }

    #endregion

    #region Properties

    public static readonly DependencyProperty TableSourceProperty =
                DependencyProperty.Register("TableSource",
                                typeof(DataGridCollectionView<DataRow>),
                                typeof(MyViewModel));
    #endregion

        #region Public Methods
        #endregion

        #region Helper Methods
        #endregion

    /// ///////////////////////////////////////////////////////////
    // Event Handling

    Task<int> OnItemsCount(object sender, EventArgs arg2)
    {
        return Task<int>.Run( () => {
            return COUNT;
        });
    }

    Task<List<DataRow>> OnItemsRequest(object sender, ItemsEventArgs arg2)
    {
        int startIndex = arg2.StartIndex;
        int count      = arg2.RequestedCount;
        SortDescription sort = arg2.SortDescription;

        var task = Task<List<DataRow>>.Run( () => {
            List<DataRow> items = new List<DataRow>();

            for (int i = startIndex; i < startIndex + count; i++) {
                DataRow row = dataTable.NewRow();

                row[0] = i.ToString();
                row[1] = rnd.Next(1000).ToString();
                row[2] = rnd.Next(1000).ToString();
                items.Add(row);
            }
            // Simulating heavy loading time
            Thread.Sleep(2 * 1000);                

            return items;
        });

        return task;
    }

        #region Events
        /// <summary>
        /// 
        /// </summary>
//        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion
    }


}
