using System;
using System.Collections;
using System.Windows.Forms;

namespace CopyToUSB2._0
{
    internal class ListViewItemComparer : IComparer
    {
        private int column;
        private SortOrder sorting;

        public ListViewItemComparer(int col, SortOrder sort)
        {
            this.column = col;
            this.sorting = sort;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = System.String.Compare(((ListViewItem)x).SubItems[column].Text, ((ListViewItem)y).SubItems[column].Text);
            if (sorting == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}