using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grepy2
{
	public partial class CollectingFiles : Form
	{
		public CollectingFiles()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Globals.GetFiles.bShouldStopCurrentJob = true;  // cancel the current GetFiles job
		}
	}
}
