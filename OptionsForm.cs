using System;
using System.Drawing;
using System.Windows.Forms;

using System.IO;

namespace Grepy2
{
	public partial class OptionsForm : Form
	{
		public bool DeferRichText
		{
			get { return this.DeferRichTextCheckBox.Checked; }
		}

		public bool UseWindowsFileAssociation
		{
			get { return this.WindowsFileAssociationCheckBox.Checked; }
		}

		public string CustomEditorText
		{
			get { return this.CustomEditorTextBox.Text.Trim(); }
		}

		public int NumberOfWorkerThreads
		{
			get { return this.WorkerThreadsComboBox.SelectedIndex + 1; }
		}

		private bool bWindowInitComplete;  // set when form window is done initializing

		private ExplorerIntegration WindowsExplorerIntegration;
		private bool bWindowsExplorerIntegrationAtStart;  // save the state of the Windows Explorer Integration check box at startup (so we can tell if it changed)

		public OptionsForm()
		{
			bWindowInitComplete = false;  // we aren't done initializing the window yet, don't overwrite any .config settings

			this.DialogResult = DialogResult.Cancel;  // set the default dialog result to 'Cancel'

			InitializeComponent();
		}

		private void OptionsForm_Load(object sender, EventArgs e)
		{
			WindowsExplorerIntegration = new ExplorerIntegration();
			bWindowsExplorerIntegrationAtStart = WindowsExplorerIntegration.IsEnabled("Directory");

			WindowsExplorerCheckBox.Checked = bWindowsExplorerIntegrationAtStart;

			int NumberOfProcessors = Math.Max(Environment.ProcessorCount - 1, 1);  // subtract one from total numberr of logical processors (so we don't max out the CPU)

			for( int index = 1; index <= NumberOfProcessors; index++ )
			{
				WorkerThreadsComboBox.Items.Add(string.Format("{0}", index));
			}

			int pos_x = -1;
			int pos_y = -1;
			if( Config.Get(Config.KEY.OptionsPosX, ref pos_x) && Config.Get(Config.KEY.OptionsPosY, ref pos_y) )
			{
				Location = new Point(pos_x, pos_y);
			}
			else  // otherwise, center the window on the main form
			{
				this.CenterToParent();
			}

			bool bDeferRichTextDisplay = false;
			Config.Get(Config.KEY.OptionsDeferRichTextDisplay, ref bDeferRichTextDisplay);
			DeferRichTextCheckBox.Checked = bDeferRichTextDisplay;

			bool bUseWindowsFileAssociation = true;
			Config.Get(Config.KEY.OptionsUseWindowsFileAssociation, ref bUseWindowsFileAssociation);
			WindowsFileAssociationCheckBox.Checked = bUseWindowsFileAssociation;

			CustomEditorTextBox.ReadOnly = bUseWindowsFileAssociation;
			CustomEditorButton.Enabled = !bUseWindowsFileAssociation;

			string CustomEditorString = "";
			if( Config.Get(Config.KEY.OptionsCustomEditor, ref CustomEditorString) )
			{
				CustomEditorTextBox.Text = CustomEditorString;
			}

			WorkerThreadsComboBox.SelectedIndex = Globals.NumWorkerThreads - 1;

			ToolTip OptionsToolTip = new ToolTip();

			OptionsToolTip.AutomaticDelay = 1000;

			OptionsToolTip.SetToolTip(this.WindowsExplorerCheckBox, "Enable or disable having Grepy2 appear in the right-click menu in Windows File Explorer when right-clicking on a folder (or drive).");

			OptionsToolTip.SetToolTip(this.label6, "The number of worker threads that you wish to use when searching through files for the search text.");
			OptionsToolTip.SetToolTip(this.WorkerThreadsComboBox, "The number of worker threads that you wish to use when searching through files for the search text.");

			OptionsToolTip.SetToolTip(this.DeferRichTextCheckBox, "Enable to defer the displaying of search match text in the RichText box until after the search is complete (this can improve performance).");

			OptionsToolTip.SetToolTip(this.WindowsFileAssociationCheckBox, "Whether you wish to use the Windows application associated with a file type to edit the file or whether you wish to specify an editor to use to edit all files.");

			bWindowInitComplete = true;  // window initialization is complete, okay to write config settings now
		}

		private void OptionsForm_Move(object sender, EventArgs e)
		{
			if( bWindowInitComplete )
			{
				Config.Set(Config.KEY.OptionsPosX, Location.X);
				Config.Set(Config.KEY.OptionsPosY, Location.Y);
			}
		}

		private void SaveConfig()
		{
			Config.Set(Config.KEY.OptionsDeferRichTextDisplay, DeferRichText);
			Config.Set(Config.KEY.OptionsUseWindowsFileAssociation, UseWindowsFileAssociation);
			Config.Set(Config.KEY.OptionsCustomEditor, CustomEditorText);
		}

		private void ValidateInput()
		{
			// verify that the Custom Editor executable exists
			bool bCustomEditorExecutableExists = false;

			string CustomEditorString = CustomEditorText;

			if( !UseWindowsFileAssociation )
			{
				if( CustomEditorString == "" )
				{
					MessageBox.Show("You must specify a custom editor executable filename if you are not using Windows file association.");
					return;
				}

				// if there's double quotes around the executable name, then get just that part (remove the filename, linenumber and other arguments)
				if( (CustomEditorString.Substring(0, 1) == "\"") && (CustomEditorString.Length > 1) )
				{
					int index = CustomEditorString.IndexOf('"', 1);

					if( index > 0 )
					{
						CustomEditorString = CustomEditorString.Substring(1, index - 1);

						// check that the executable file exists
						if( File.Exists(CustomEditorString) )
						{
							bCustomEditorExecutableExists = true;
						}
					}

					if( !bCustomEditorExecutableExists )
					{
						string msg = string.Format("Custom Editor executable '{0}' does not exist\n\n(or double quotes around the executable name are set up incorrectly.)", CustomEditorString);
						MessageBox.Show(msg);
						return;
					}
				}
				else
				{
					// otherwise, if there's no double quotes around the executable name, then check for space before arguments
					int space_index = CustomEditorString.IndexOf(" ", 0);

					if( space_index > 0 )
					{
						CustomEditorString = CustomEditorString.Substring(0, space_index);
					}

					if( !File.Exists(CustomEditorString) )
					{
						string msg = string.Format("Custom Editor executable '{0}' does not exist\n\n(or it is missing double quotes around it or there is no space before the filename or line number arguments.)", CustomEditorString);
						MessageBox.Show(msg);
						return;
					}
				}
			}

			if( bWindowsExplorerIntegrationAtStart != WindowsExplorerCheckBox.Checked )
			{
				// set windows explorer integration
				if( WindowsExplorerIntegration != null )
				{
					WindowsExplorerIntegration.Set( Globals.ApplicationPathExe, WindowsExplorerCheckBox.Checked );
				}
			}

			SaveConfig();

			this.DialogResult = DialogResult.OK;

			Close();
		}

		private void OptionsOkButton_Click(object sender, EventArgs e)
		{
			ValidateInput();
		}

		private void OptionsHelpButton_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, "Grepy2Help.chm", HelpNavigator.TopicId, "2");
		}

		private void OptionsCancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void WindowsFileAssociationCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CustomEditorTextBox.ReadOnly = WindowsFileAssociationCheckBox.Checked;
			CustomEditorButton.Enabled = !WindowsFileAssociationCheckBox.Checked;
		}

		private void CustomEditorButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.Filter = "Executables (.exe)|*.exe|Command Files (*.cmd)|*.cmd|All Files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;

			openFileDialog.Multiselect = false;

			if( openFileDialog.ShowDialog() == DialogResult.OK )
			{
				CustomEditorTextBox.Text = "\"" + openFileDialog.FileName + "\"";
			}
		}
	}
}
