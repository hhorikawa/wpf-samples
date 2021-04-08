using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataVirtualization
{
    /// <summary>
    /// Represents a provider of collection details.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
public interface IItemsProvider<T>
{
    // @return The total number of items.
    Task<int> Count();

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
    Task<IList<T> > GetRange(int startIndex, int count);
}

}
