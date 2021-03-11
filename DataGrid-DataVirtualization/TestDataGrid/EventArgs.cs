
using System;
using System.ComponentModel; // SortDescription

namespace TestDataGrid
{

public class ItemsEventArgs: EventArgs
{
    public ItemsEventArgs(int startIndex, int requestedCount, 
                          SortDescription sortDescription)
    {
        StartIndex = startIndex;
        RequestedCount = requestedCount;
        SortDescription = sortDescription;
    }

    public int RequestedCount { get; protected set; }
    public int StartIndex { get; protected set; }
    public SortDescription SortDescription { get; protected set; }

} // class ItemsEventArgs

}


