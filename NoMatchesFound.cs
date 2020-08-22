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
	public partial class NoMatchesFound : Form
	{
		public NoMatchesFound()
		{
			InitializeComponent();

			this.StartPosition = FormStartPosition.CenterParent;
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
