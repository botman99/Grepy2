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
	public partial class PleaseWait : Form
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		public PleaseWait()
		{

			InitializeComponent();
		}

		private void PleaseWait_Shown(object sender, EventArgs e)
		{
			progressBar1.Value = 0;

			Application.DoEvents();  // this will cause the form to fully update itself before continuing (so that everything is fully rendered)

			Globals.bPleaseWaitDialogCancelled = false;

			HandleRef FormHandle = new HandleRef(this, Globals.MainFormHandle);

			PostMessage(FormHandle, Globals.WM_PLEASEWAITSHOWN_NOTIFY_MAIN_THREAD, IntPtr.Zero, IntPtr.Zero);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Globals.bPleaseWaitDialogCancelled = true;
		}

		public void UpdateProgressBar(int value)
		{
			progressBar1.Value = value;
		}
	}
}
