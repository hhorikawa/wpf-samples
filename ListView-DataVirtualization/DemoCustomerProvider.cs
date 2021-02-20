using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DataVirtualization
{
    /// <summary>
    /// Demo implementation of IItemsProvider returning dummy customer items after
    /// a pause to simulate network/disk latency.
    /// </summary>
public class DemoCustomerProvider : IItemsProvider<Customer>
{
    private readonly int _count;
    private readonly int _fetchDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoCustomerProvider"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="fetchDelay">The fetch delay.</param>
    public DemoCustomerProvider(int count, int fetchDelay)
    {
        _count = count;
        _fetchDelay = fetchDelay;
    }

    // @return The total number of items.
    public Task<int> Count()
    {
        Trace.WriteLine("FetchCount");
        return Task.Run(() => {
            Thread.Sleep(_fetchDelay);
            return _count;
        });
    }

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
    public Task<IList<Customer> > GetRange(int startIndex, int count)
    {
        Trace.WriteLine("FetchRange: "+startIndex+","+count);

        return Task.Run( () => { 
            Thread.Sleep(_fetchDelay);

            IList<Customer> list = new List<Customer>();
            for( int i = startIndex; i < startIndex + count; i++ ) {
                Customer customer = new Customer {Id = i+1, Name = "Customer " + (i+1)};
                list.Add(customer);
            }
            return list;
        });
    }
} // class DemoCustomerProvider

}

