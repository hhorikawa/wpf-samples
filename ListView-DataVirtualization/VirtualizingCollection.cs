
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/**
 * ListView.ItemsSource は IEnumerable 型. <-かなり基本的な型. 何でも使える感じ.
 *   ... IEnumerable<T> ではないことに注意.
 *
 * DataGrid では, IEnumerable インタフェイスだけでは不十分で, 非ジェネリックな
 * IList を実装していなければならない
 *
 * IList<T> implements ICollection<T>. ICollection<T> implements IEnumerable<T>.
 * IEnumerable<T> implements IEnumerable.
 * IList implements ICollection. ICollection implements IEnumerable.
 *   => 過去との互換性のため, 標準クラスでは IList<T>, IList の両方を実装するク
 *      ラスが多いが, 単に IList を実装するのでもよい.
 *
 * DataGrid で並び替え、フィルタしたいときは, コンテナオブジェクトを
 * CollectionView で wrap する. しかし、そもそも実データが全部ない場合は,
 * CollectionView の意味がない。
 *
 * ItemsSource に渡す具象クラスとしては, CLR List<T> よりも
 * ObservableCollection<T> が推奨される.
 *   See https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/optimizing-performance-data-binding
 * しかし, クラスを自作するときは, ObservableCollection<T> からは難儀。
 *   => 非ジェネリックな IList インタフェイスを実装して, Collection<T> class を
 *      作るような形にするのが良い。
 */


namespace DataVirtualization
{

    /// Specialized list implementation that provides data virtualization. The collection is divided up into pages,
    /// and pages are dynamically fetched from the IItemsProvider when required. Stale pages are removed after a
    /// configurable period of time.
    /// Intended for use with large collections on a network or disk resource that cannot be instantiated locally
    /// due to memory consumption or fetch latency.
    /// </summary>
    /// <remarks>
    /// The IList implmentation is not fully complete, but should be sufficient for use as read only collection
    /// data bound to a suitable ItemsControl.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
// このインスタンスを ItemsSource に設定している。
// ListCollectionView に渡せるよう, 非ジェネリックな IList のみを実装する.
public class VirtualizingCollection<T> : IList
{
    const string READONLY_ERROR = "Read Only";

    // 実際にデータを取得するクラス。ネットワーク経由、など。
    protected readonly IItemsProvider<T> _itemsProvider;

    // 疎な配列.
    protected readonly Dictionary<int, IList<T> > _pages =
                                        new Dictionary<int, IList<T> >();

    protected readonly Dictionary<int, DateTime> _pageTouchTimes =
                                        new Dictionary<int, DateTime>();

    // 1ページ当たりの要素数
    protected readonly int _pageSize;

    private Object _syncRoot;


    ///////////////////////////////////////////////////////////
    #region Constructors

    /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
    /// <param name="pageSize">Size of the page.</param>
    public VirtualizingCollection(IItemsProvider<T> itemsProvider,
                                  int pageSize = 1000)
    {
        if (itemsProvider == null)
            throw new ArgumentNullException("itemsProvider");
        if (pageSize < 0 )
            throw new ArgumentOutOfRangeException("pageSize");

        _itemsProvider = itemsProvider;
        _pageSize = pageSize;
    }

    #endregion


    ///////////////////////////////////////////////////////////
    #region Public Properties

    protected int _count = -1;
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// The first time this property is accessed, it will fetch the count from the IItemsProvider.
    // @return The number of the virtualized total elements. <see cref="T:System.Collections.Generic.ICollection`1"/>.
    public virtual int Count {
        get {
            if (_count == -1) {
                _count = _itemsProvider.Count().Result; // 同期する
            }
            return _count;
        }
    }

    /// Gets the item at the specified index. This property will fetch
    /// the corresponding page from the IItemsProvider if required.
    /// </summary>
    /// <value></value>
    // このプロパティがキモ!
    public object this[int index] {
        get {
            // determine which page and offset within page
            int pageIndex = index / _pageSize;
            int pageOffset = index % _pageSize;

            // request primary page
            RequestPage(pageIndex);

            // if accessing upper 50% then request next page
            if ( pageOffset > _pageSize / 2 && pageIndex < Count / _pageSize)
                RequestPage(pageIndex + 1);
            else if (pageOffset < _pageSize / 2 && pageIndex > 0) {
                // if accessing lower 50% then request prev page
                RequestPage(pageIndex - 1);
            }

            // remove stale pages
            CleanUpPages();

            // defensive check in case of async load
            if (_pages[pageIndex] == null)
                return default(T);

            // return requested item
            return _pages[pageIndex][pageOffset];
        }
        set { throw new NotSupportedException(READONLY_ERROR); }
    }

    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
    /// </summary>
    /// <value></value>
    /// <returns>Always true.
    public bool IsReadOnly {
        get { return true; }
    }

    /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
    /// </summary>
    /// <value></value>
    /// <returns>Always false.
    /// </returns>
    public bool IsFixedSize {
        get { return true; }
    }

    /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
    public object SyncRoot {
        get {
            if (_syncRoot == null) {
                ICollection c = _pages as ICollection;
                _syncRoot = c.SyncRoot;
            }
            return _syncRoot;
        }
    }

    /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
    /// </summary>
    /// <value></value>
    /// <returns>Always false.
    public bool IsSynchronized {
        get { return false; }
    }

    #endregion


    //////////////////////////////////////////////////////////////////
    #region Public Methods

    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <remarks>
    /// This method should be avoided on large collections due to poor performance.
    /// </remarks>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < Count; i++) {
            yield return this[i];
        }
    }

    /// Not supported.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
    /// <exception cref="T:System.NotSupportedException">
    /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
    public int Add(object value)
    {
        throw new NotSupportedException(READONLY_ERROR);
    }


    /// Not supported.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
    /// <returns>
    /// Always false.
    public bool Contains(object value)
    {
        throw new NotSupportedException();
    }


    /// Not supported.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
    public void Clear()
    {
        throw new NotSupportedException(READONLY_ERROR);
    }


    /// Not supported
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
    /// <returns>
    /// Always -1.
    public int IndexOf(object value)
    {
        return -1; // dummy
        throw new NotSupportedException();
    }


    /// Not supported.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
    /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
    public void Insert(int index, object value)
    {
        throw new NotSupportedException(READONLY_ERROR);
    }


    /// Not supported.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
    public void RemoveAt(int index)
    {
        throw new NotSupportedException(READONLY_ERROR);
    }


    /// Not supported.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
    /// <returns>
    /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
    public void Remove(object value)
    {
        throw new NotSupportedException(READONLY_ERROR);
    }


    // this の先頭から全部を array[index] 以降にコピーする.
    /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// 	<paramref name="array"/> is null.
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// 	<paramref name="arrayIndex"/> is less than 0.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="array"/> is multidimensional.
    /// -or-
    /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
    /// -or-
    /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
    /// -or-
    /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
    public void CopyTo(Array array, int index)
    {
        if (array == null)
            throw new ArgumentNullException("array");
        if (array.Rank != 1)
            throw new ArgumentException("array.Rank");
        if (index < 0)
            throw new ArgumentOutOfRangeException("index");
        if (array.Length - index < Count)
            throw new ArgumentException("array size");

        object[] objects = array as object[];
        int count = Count;
        for (int i = 0; i < count; i++)
            objects[index++] = this[i];
    }

    #endregion


    //////////////////////////////////////////////////////////////////
    #region Page Caching

    /// Cleans up any stale pages that have not been accessed in the period dictated by PageTimeout.
    private void CleanUpPages()
    {
        // TODO: Least Recently Used (LRU) で破棄すること。
        //       Now との比較でしきい値は無いわー
/*
            List<int> keys = new List<int>(_pageTouchTimes.Keys);
            foreach (int key in keys)
            {
                // page 0 is a special case, since WPF ItemsControl access the first item frequently
                if ( key != 0 && (DateTime.Now - _pageTouchTimes[key]).TotalMilliseconds > PageTimeout )
                {
                    _pages.Remove(key);
                    _pageTouchTimes.Remove(key);
                    Trace.WriteLine("Removed Page: " + key);
                }
            }
    */
    }

/*
        /// <summary>
        /// Populates the page within the dictionary.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="page">The page.</param>
        protected virtual void PopulatePage(int pageIndex, IList<T> page)
        {
            Trace.WriteLine("Page populated: "+pageIndex);
            if ( _pages.ContainsKey(pageIndex) )
                _pages[pageIndex] = page;
        }
*/


    /// Makes a request for the specified page, creating the necessary slots in the dictionary,
    /// and updating the page touch time.
    /// </summary>
    /// <param name="pageIndex">Index of the page.</param>
    private void RequestPage(int pageIndex)
    {
        if (!_pages.ContainsKey(pageIndex)) {
            _pages.Add(pageIndex, null); // 非同期のため, ガードする. TODO: ロック
            LoadPage(pageIndex);
            Trace.WriteLine("Added page: " + pageIndex);
        }
        else { 
            _pageTouchTimes[pageIndex] = DateTime.Now;
        }
    }

    // TODO: フィルタ, 並べ替え.
    protected virtual void LoadPage(int pageIndex)
    {
        // 同期する
        _pages[pageIndex] =
                 _itemsProvider.GetRange(pageIndex * _pageSize, _pageSize).Result;
        _pageTouchTimes[pageIndex] = DateTime.Now;
    }

    #endregion
}

}
