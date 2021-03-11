
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TestDataGrid
{

// 型定義
public delegate Task<List<T>> ItemsRequestEventHandler<T>(
                                            object sender, ItemsEventArgs e);

public class DataGridCollectionView<T> : ListCollectionView
{
    #region Events
    // Action<T1, T2> delegate 型の戻り値の型は void.
    // 戻り値を取りたいときは Func<> を使う。
    // "event" キーワードは、型を修飾し、宣言元のクラス内からしか呼び出せなく
    // する.
    public event Func<object, EventArgs, Task<int>> ItemsCount;
    public event ItemsRequestEventHandler<T> ItemsRequest;
    #endregion

    /// ///////////////////////////////////////////////////////////////
    // Fields

    readonly int _pageSize;

    // Page table.
    Dictionary<int, List<T>> _dataRows = new Dictionary<int, List<T>>();

    /// ///////////////////////////////////////////////////////////////
    // Constructors

    public DataGridCollectionView(int pageSize = 500) : 
        base(new List<object>())  // dummy
    {
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException();
        _pageSize = pageSize;
    }


    /// ///////////////////////////////////////////////////////////////
    // Properties

    int _count = -1;
    public override int Count {
        get {
            if (_count == -1) {
                Task<int> t = ItemsCount(this, new EventArgs());
                _count = t.Result; // 待つ。手抜き。
            }
            return _count;
        }
    }


    /// ///////////////////////////////////////////////////////////////
    // Public Methods

    public void RefreshDataRows()
    {
        _dataRows.Clear();
    }

    public override object GetItemAt(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException();

        int page_num = index / _pageSize;
        request_page(page_num);

        // 非同期 load 対策
        var page_data = _dataRows[page_num];
        return page_data == null ? default(T) : page_data[index % _pageSize];
    }

    /// ///////////////////////////////////////////////////////////////
    // Implementation

    //bool m_lock = false;
    void request_page(int page_num)
    {
        if (!_dataRows.ContainsKey(page_num)) {
            //if (m_lock)
            //    return; // 動かす task は一つだけ.

            _dataRows.Add(page_num, null); // ガードする
            //m_lock = true;

            ItemsEventArgs args = new ItemsEventArgs(
                    _pageSize * page_num,  // startIndex
                    _pageSize,             // requestedCount
                    SortDescriptions.FirstOrDefault() );
            Task<List<T>> task = ItemsRequest(this, args);
            //items_task.ContinueWith(task => {  ●●これを活かすとクラッシュする
                                                 // TODO: ちゃんとする
                _dataRows[page_num] = task.Result; // 待つ. 手抜き.
                Console.WriteLine("Updated: " + page_num);

                Action action = () => this.Refresh();
                Application.Current.Dispatcher.BeginInvoke(action);

                //m_lock = false;
            //});
        }
    }


        #region Helper Methods
        #endregion

        #region Event Handling
        #endregion
    }
}
