using System;
using System.Collections;
using System.Collections.Generic;
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
            string a = ((ListViewItem)x).SubItems[column].Text;
            string b = ((ListViewItem)y).SubItems[column].Text;
            if (this.column==2)
            {
                returnVal = (Format.Bytes(a) < Format.Bytes(b)) ? 1 : -1;
            }else
            {
                returnVal = System.String.Compare(a, b);
            }
            if (sorting == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}