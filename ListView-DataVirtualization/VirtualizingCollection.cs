﻿#define USE_ILIST // こっちが必要!

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataVirtualization
{
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
public class VirtualizingCollection<T> : //IList<T>, 
#if USE_ILIST
    IList
#endif
   // , IReadOnlyList<T> これも有効にするとフリーズしてしまう. Why?
{
    #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageTimeout">The page timeout.</param>
    public VirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize, int pageTimeout)
    {
            _itemsProvider = itemsProvider;
            _pageSize = pageSize;
            _pageTimeout = pageTimeout;
    }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
    public VirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize)
        {
            _itemsProvider = itemsProvider;
            _pageSize = pageSize;
    }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
    public VirtualizingCollection(IItemsProvider<T> itemsProvider)
        {
            _itemsProvider = itemsProvider;
    }

    #endregion

    //////////////////////////////////////////////////////////////////
    #region Properties

    private readonly IItemsProvider<T> _itemsProvider;
            /// <summary>
        /// Gets the items provider.
        /// </summary>
        /// <value>The items provider.</value>
    public IItemsProvider<T> ItemsProvider {
            get { return _itemsProvider; }
    }


    private readonly int _pageSize = 100;
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
    public int PageSize {
            get { return _pageSize; }
    }

    private readonly long _pageTimeout = 10000;
        /// <summary>
        /// Gets the page timeout.
        /// </summary>
        /// <value>The page timeout.</value>
    public long PageTimeout {
            get { return _pageTimeout; }
    }

    #endregion

    //////////////////////////////////////////////////////////////////
    #region IList<T>, IList properties

    private int _count = -1;
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// The first time this property is accessed, it will fetch the count from the IItemsProvider.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
    public virtual int Count {
        get {
            if (_count == -1)
                    LoadCount();
            return _count;
        }
        protected set {
                _count = value;
        }
    }


        /// <summary>
        /// Gets the item at the specified index. This property will fetch
        /// the corresponding page from the IItemsProvider if required.
        /// </summary>
        /// <value></value>
    // このプロパティがキモ!
    public T this[int index] {
        get {
                // determine which page and offset within page
                int pageIndex = index / PageSize;
                int pageOffset = index % PageSize;

                // request primary page
                RequestPage(pageIndex);

                // if accessing upper 50% then request next page
                if ( pageOffset > PageSize/2 && pageIndex < Count / PageSize)
                    RequestPage(pageIndex + 1);

                // if accessing lower 50% then request prev page
                if (pageOffset < PageSize/2 && pageIndex > 0)
                    RequestPage(pageIndex - 1);

                // remove stale pages
                CleanUpPages();

                // defensive check in case of async load
                if (_pages[pageIndex] == null)
                    return default(T);

                // return requested item
                return _pages[pageIndex][pageOffset];
            }
            set { throw new NotSupportedException(); }
        }

#if USE_ILIST
    object IList.this[int index] {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
    }
#endif

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>Always true.
        /// </returns>
    public bool IsReadOnly {
            get { return true; }
    }

    #endregion

    //////////////////////////////////////////////////////////////////
    #region IEnumerable<T>, IEnumerable methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>
        /// This method should be avoided on large collections due to poor performance.
        /// </remarks>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
    public IEnumerator<T> GetEnumerator() 
    {
        for (int i = 0; i < Count; i++) {
            yield return this[i];
        }
    }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
    public void Add(T item)
    {
            throw new NotSupportedException();
    }

#if USE_ILIST
        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }
#endif

#if USE_ILIST
        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }
#endif

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// Always false.
        /// </returns>
        public bool Contains(T item)
        {
            return false;
        }


        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }


#if USE_ILIST
        int IList.IndexOf(object value)
        {
            return IndexOf((T) value);
        }
#endif
        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// Always -1.
        /// </returns>
        public int IndexOf(T item)
        {
            return -1;
        }


        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

#if USE_ILIST
        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }
#endif

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

#if USE_ILIST
        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }
#endif
        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Not supported.
        /// </summary>
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
        /// </exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
            throw new NotSupportedException();
    }

#if USE_ILIST
        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }
#endif
    #endregion
    
        
    #region Misc

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsFixedSize
        {
            get { return false; }
        }

    #endregion

        #region Paging

        private readonly Dictionary<int, IList<T>> _pages = new Dictionary<int, IList<T>>();
        private readonly Dictionary<int, DateTime> _pageTouchTimes = new Dictionary<int, DateTime>();

        /// <summary>
        /// Cleans up any stale pages that have not been accessed in the period dictated by PageTimeout.
        /// </summary>
        public void CleanUpPages()
        {
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
        }

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

        /// <summary>
        /// Makes a request for the specified page, creating the necessary slots in the dictionary,
        /// and updating the page touch time.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void RequestPage(int pageIndex)
        {
            if (!_pages.ContainsKey(pageIndex))
            {
                _pages.Add(pageIndex, null);
                _pageTouchTimes.Add(pageIndex, DateTime.Now);
                Trace.WriteLine("Added page: " + pageIndex);
                LoadPage(pageIndex);
            }
            else
            {
                _pageTouchTimes[pageIndex] = DateTime.Now;
            }
        }

        #endregion

        #region Load methods

        /// <summary>
        /// Loads the count of items.
        /// </summary>
        protected virtual void LoadCount()
        {
            Count = FetchCount();
        }

        /// <summary>
        /// Loads the page of items.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void LoadPage(int pageIndex)
        {
            PopulatePage(pageIndex, FetchPage(pageIndex));
        }

        #endregion

        #region Fetch methods

        /// <summary>
        /// Fetches the requested page from the IItemsProvider.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        protected IList<T> FetchPage(int pageIndex)
        {
            return ItemsProvider.FetchRange(pageIndex*PageSize, PageSize);
        }

        /// <summary>
        /// Fetches the count of itmes from the IItemsProvider.
        /// </summary>
        /// <returns></returns>
        protected int FetchCount()
        {
            return ItemsProvider.FetchCount();
        }

        #endregion
    }
}
