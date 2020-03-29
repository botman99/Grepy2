using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace Grepy2
{
	public partial class BetterRichTextBox : RichTextBox
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		const int WM_LBUTTONDOWN = 0x0201;
		const int WM_LBUTTONDBLCLK = 0x0203;
		const int WM_RBUTTONDOWN = 0x0204;

		private bool bIsFirstClick = false;
		private bool bWasCtrlPressedWhenClicked = false;

		public HandleRef FormHandle = new HandleRef();
		public Point RichTextMousePosition;  // saves the position of the most recent mouse click location in the RichTextBox

		private System.Timers.Timer doubleClickTimer = new System.Timers.Timer();


		public BetterRichTextBox()
		{
			InitializeComponent();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			// fix bug with AutoWordSelection (it is true by default but setting it to 'false' won't turn it off, you have to enable it then disable it to turn it off)
			this.AutoWordSelection = true;
			this.AutoWordSelection = false;

			doubleClickTimer.SynchronizingObject = this;
			doubleClickTimer.Elapsed += new System.Timers.ElapsedEventHandler(doubleClickTimerEvent);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
		}

		private bool IsValidSelectionCharacter(char c)
		{
			if (bWasCtrlPressedWhenClicked && ((ModifierKeys & Keys.Control) == Keys.Control))
			{
				if (!Char.IsWhiteSpace(c))
				{
					return true;
				}
			}

			if (Char.IsLetterOrDigit(c) || (c == '_'))
			{
				return true;
			}

			return false;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_LBUTTONDBLCLK)
			{
				if (this.Text.Length > 0)
				{
					int start_index = Math.Min(this.SelectionStart, this.Text.Length - 1);
					int original_start_index = start_index;
					int end_index = start_index;

					while ((start_index >= 0) && IsValidSelectionCharacter(this.Text[start_index]))
					{
						start_index--;
					}

					if (start_index != original_start_index)
					{
						start_index++;
					}

					while ((end_index < this.Text.Length) && IsValidSelectionCharacter(this.Text[end_index]))
					{
						end_index++;
					}

					if (end_index != original_start_index)
					{
						end_index--;
					}

					this.SelectionStart = start_index;
					this.SelectionLength = end_index - start_index + 1;
				}

				doubleClickTimer.Enabled = false;

				bIsFirstClick = false;
				bWasCtrlPressedWhenClicked = false;

				return;
			}
			else if (m.Msg == WM_LBUTTONDOWN)
			{
				int xPos = (int)m.LParam & 0xffff;  // get low word
				int yPos = ((int)m.LParam >> 16) & 0xffff;  // get high word

				RichTextMousePosition = new Point(xPos, yPos);

				if (!bIsFirstClick)
				{
					bIsFirstClick = true;

					if( (ModifierKeys & Keys.Control) == Keys.Control )
					{
						bWasCtrlPressedWhenClicked = true;
					}

					doubleClickTimer.Interval = SystemInformation.DoubleClickTime;
					doubleClickTimer.Enabled = true;
				}
				else
				{
					bIsFirstClick = false;  // we have seen two left clicks before the doubleClickTimer timed out, this is not a single click event (we should never get here)
					bWasCtrlPressedWhenClicked = false;
				}
			}
			else if (m.Msg == WM_RBUTTONDOWN)
			{
				int xPos = (int)m.LParam & 0xffff;  // get low word
				int yPos = ((int)m.LParam >> 16) & 0xffff;  // get high word

				RichTextMousePosition = new Point(xPos, yPos);
			}

			base.WndProc(ref m);
		}

		private void doubleClickTimerEvent(object source, System.Timers.ElapsedEventArgs e)
		{
			doubleClickTimer.Enabled = false;

			if (bIsFirstClick)
			{
				if (SelectionLength == 0)  // was nothing selected between mouse down and mouse up event?
				{
					if( bWasCtrlPressedWhenClicked )
					{
						if (FormHandle.Handle != IntPtr.Zero)
						{
							PostMessage(FormHandle, Globals.WM_CTRL_CLICK_NOTIFY_MAIN_THREAD, IntPtr.Zero, IntPtr.Zero);  // notify main Form thread that ctrl single click was detected
						}
					}
				}

				bIsFirstClick = false;  // double click wasn't seen within the timeout, clear the bFirstClick bool
				bWasCtrlPressedWhenClicked = false;
			}
		}

	}
}
