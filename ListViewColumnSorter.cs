using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;

namespace Grepy2
{
	// from http://support.microsoft.com/kb/319401 (with modifications for String.Compare)

	public class ListViewColumnSorter : IComparer
	{
		const Int32 HDF_SORTDOWN	= 0x200;
		const Int32 HDF_SORTUP		= 0x400;
		const Int32 HDI_FORMAT		= 0x4;
		const Int32 HDM_GETITEM		= 0x120b;
		const Int32 HDM_SETITEM		= 0x120c;
		const Int32 LVM_GETHEADER	= 0x101f;

		[StructLayout(LayoutKind.Sequential)]
		public struct LVCOLUMN
		{
			public Int32 mask;
			public Int32 cx;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string pszText;
			public IntPtr hbm;
			public Int32 cchTextMax;
			public Int32 fmt;
			public Int32 iSubItem;
			public Int32 iImage;
			public Int32 iOrder;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		static extern IntPtr SendMessageLVCOLUMN(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref LVCOLUMN lParam);

		private int ColumnToSort;
		private SortOrder OrderOfSort;
		private bool bCaseSensitiveColumnSort;

		public ListViewColumnSorter()
		{
			// Initialize the column to '0'
			ColumnToSort = 0;

			// Initialize the sort order to 'none'
			OrderOfSort = SortOrder.None;
		}

		public void SetCaseSensitiveColumnSort(bool bInCaseSensitive)
		{
			bCaseSensitiveColumnSort = bInCaseSensitive;
		}

		public int Compare(object x, object y)
		{
			int compareResult;
			ListViewItem listviewX, listviewY;

			// Cast the objects to be compared to ListViewItem objects
			listviewX = (ListViewItem)x;
			listviewY = (ListViewItem)y;

			// Compare the two items
			if( (ColumnToSort == 3) || (ColumnToSort == 4) )  // 'Matches' or 'Filesize', sort as numbers
			{
				string x_str = listviewX.SubItems[ColumnToSort].Text.ToString();
				string y_str = listviewY.SubItems[ColumnToSort].Text.ToString();
				int x_value = System.Convert.ToInt32(x_str);
				int y_value = System.Convert.ToInt32(y_str);
				compareResult = (x_value - y_value);
			}
			else
			{
				if( bCaseSensitiveColumnSort )
				{
					compareResult = String.Compare(listviewX.SubItems[ColumnToSort].Text,listviewY.SubItems[ColumnToSort].Text, StringComparison.Ordinal);  // case sensitive
				}
				else
				{
					compareResult = String.Compare(listviewX.SubItems[ColumnToSort].Text,listviewY.SubItems[ColumnToSort].Text, StringComparison.OrdinalIgnoreCase);  // case insensitive
				}
			}
			
			// Calculate correct return value based on object comparison
			if( OrderOfSort == SortOrder.Ascending )
			{
				// Ascending sort is selected, return normal result of compare operation
				return compareResult;
			}
			else if( OrderOfSort == SortOrder.Descending )
			{
				// Descending sort is selected, return negative result of compare operation
				return (-compareResult);
			}
			else
			{
				// Return '0' to indicate they are equal
				return 0;
			}
		}

		public int SortColumn
		{
			set { ColumnToSort = value; }
			get { return ColumnToSort; }
		}

		public SortOrder Order
		{
			set { OrderOfSort = value; }
			get { return OrderOfSort; }
		}

		public void SetSortIcon(ListView listview)
		{
            IntPtr clmHdr = SendMessage(listview.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            for (int i = 0; i < listview.Columns.Count; i++)
            {
                IntPtr clmPtr = new IntPtr(i);
                LVCOLUMN lvColumn = new LVCOLUMN();

                lvColumn.mask = HDI_FORMAT;
                SendMessageLVCOLUMN(clmHdr, HDM_GETITEM, clmPtr, ref lvColumn);

                if (Order != SortOrder.None && i == ColumnToSort)
                {
                    if (Order == SortOrder.Ascending)
                    {
                        lvColumn.fmt &= ~HDF_SORTDOWN;
                        lvColumn.fmt |= HDF_SORTUP;
                    }
                    else
                    {
                        lvColumn.fmt &= ~HDF_SORTUP;
                        lvColumn.fmt |= HDF_SORTDOWN;
                    }
                }
                else
                {
                    lvColumn.fmt &= ~HDF_SORTDOWN & ~HDF_SORTUP;
                }

                SendMessageLVCOLUMN(clmHdr, HDM_SETITEM, clmPtr, ref lvColumn);
            }
		}
	}
}
