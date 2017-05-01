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
	public partial class SearchHelpForm : Form
	{
		private bool bWindowInitComplete;  // set form window is done initializing

		public SearchHelpForm()
		{
			bWindowInitComplete = false;  // we aren't done initializing the window yet, don't overwrite any .config settings

			InitializeComponent();
		}

		private void SearchHelpForm_Load(object sender, EventArgs e)
		{
			int pos_x = -1;
			int pos_y = -1;
			if( Config.Get(Config.KEY.SearchHelpPosX, ref pos_x) && Config.Get(Config.KEY.SearchHelpPosY, ref pos_y) )
			{
				Location = new Point(pos_x, pos_y);
			}
			else  // otherwise, center the window on the main form
			{
				this.CenterToParent();
			}

			bWindowInitComplete = true;  // window initialization is complete, okay to write config settings now
		}

		private void SearchHelpForm_Closing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;  // don't let the 'X' button close the form (the SearchForm will close this form)
			Hide();
		}

		private void SearchHelpForm_Move(object sender, EventArgs e)
		{
			if( bWindowInitComplete )
			{
				Config.Set(Config.KEY.SearchHelpPosX, Location.X);
				Config.Set(Config.KEY.SearchHelpPosY, Location.Y);
			}
		}

		private void SearchHelpMoreButton_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, "Grepy2Help.chm", HelpNavigator.TopicId, "1");
		}

		private void SearchHelpOkButton_Click(object sender, EventArgs e)
		{
			Hide();
		}

		private void SearchHelpForm_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.LinkText);
		}
	}
}
