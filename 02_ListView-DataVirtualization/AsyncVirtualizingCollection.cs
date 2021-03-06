using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace DataVirtualization
{
    /// <summary>
    /// Derived VirtualizatingCollection, performing loading asychronously.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
public class AsyncVirtualizingCollection<T> : VirtualizingCollection<T>,
        INotifyCollectionChanged, INotifyPropertyChanged
{
    ///////////////////////////////////////////////////////////
    // Constructors

    /// Initializes a new instance.
    /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
    public AsyncVirtualizingCollection(IItemsProvider<T> itemsProvider,
                                       int pageSize = 1000)
            : base(itemsProvider, pageSize)
    {
        _synchronizationContext = SynchronizationContext.Current;
    }


    private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Gets the synchronization context used for UI-related operations. This is obtained as
        /// the current SynchronizationContext when the AsyncVirtualizingCollection is created.
        /// </summary>
        /// <value>The synchronization context.</value>
    protected SynchronizationContext UiThreadContext
    {
        get { return _synchronizationContext; }
    }


        #region INotifyCollectionChanged

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Fires the collection reset event.
        /// </summary>
        private void FireCollectionReset()
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(e);
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler h = PropertyChanged;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Fires the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void FirePropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
            OnPropertyChanged(e);
        }

        #endregion


    private bool _isLoading;
        /// <summary>
        /// Gets or sets a value indicating whether the collection is loading.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this collection is loading; otherwise, <c>false</c>.
        /// </value>
    public bool IsLoading {
        get {
                return _isLoading;
        }
        set {
                if ( value != _isLoading )
                {
                    _isLoading = value;
                }
                FirePropertyChanged("IsLoading");
        }
    }

        #region Load overrides

        /// <summary>
        /// Asynchronously loads the count of items.
        /// </summary>
    public override int Count {
        get {
            if (_count == -1) {
                _count = 0; // TODO: ロック
                IsLoading = true;
                var task = _itemsProvider.Count();
                task.ContinueWith( t => {
                    UiThreadContext.Send(LoadCountCompleted, t.Result);
                });
            }
            return _count;
        }
    }

/*
        /// <summary>
        /// Performed on background thread.
        /// </summary>
        /// <param name="args">None required.</param>
        private void LoadCountWork(object args)
        {
            int count = FetchCount();
            SynchronizationContext.Send(LoadCountCompleted, count);
        }
*/

        /// <summary>
        /// Performed on UI-thread after LoadCountWork.
        /// </summary>
        /// <param name="args">Number of items returned.</param>
    private void LoadCountCompleted(object args)
    {
        _count = (int) args;
        IsLoading = false;
        FireCollectionReset();
    }


        /// <summary>
        /// Asynchronously loads the page.
        /// </summary>
        /// <param name="index">The index.</param>
    protected override void LoadPage(int pageIndex)
    {
        IsLoading = true;
        var task = _itemsProvider.GetRange(pageIndex * _pageSize, _pageSize);
        task.ContinueWith( t => {
            // Dictionary<> が thread-safe ではないので, UI thread でコールバック
            UiThreadContext.Send(LoadPageCompleted,
                                new object[] { pageIndex, t.Result} );
        });
    }
/*
        /// <summary>
        /// Performed on background thread.
        /// </summary>
        /// <param name="args">Index of the page to load.</param>
        private void LoadPageWork(object args)
        {
            int pageIndex = (int)args;
            IList<T> page = FetchPage(pageIndex);
            SynchronizationContext.Send(LoadPageCompleted,
                                    new object[]{ pageIndex, page });
        }
*/

    // Performed on UI-thread after LoadPageWork.
        /// </summary>
        /// <param name="args">object[] { int pageIndex, IList(T) page }</param>
    private void LoadPageCompleted(object args)
    {
        int pageIndex = (int) ((object[]) args)[0];
        IList<T> page = (IList<T>) ((object[]) args)[1];

        _pages[pageIndex] = page;
        _pageTouchTimes[pageIndex] = DateTime.Now;

        IsLoading = false;
        FireCollectionReset();
    }

    #endregion
}

}
