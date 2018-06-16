using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System.IO;
using System.Text.RegularExpressions;

namespace Grepy2
{
	public partial class SearchForm : Form
	{
		const int NUM_SEARCH_HISTORY = 30;
		const int NUM_FILESPEC_HISTORY = 20;
		const int NUM_FOLDER_HISTORY = 20;

		public string SearchFor
		{
			get { return this.SearchForComboBox.Text; }
		}

		public string FileSpec
		{
			get { return this.FileSpecComboBox.Text.Trim(); }
		}

		public string FolderName
		{
			get { return this.FolderComboBox.Text.Trim(); }
		}

		public bool bRegularExpression
		{
			get { return this.RegExCheckBox.Checked; }
		}

		public bool bMatchCase
		{
			get { return MatchCaseCheckBox.Checked; }
		}

		public bool bIsRecursive
		{
			get { return this.RecurseFolderCheckBox.Checked; }
		}

		private bool bWindowInitComplete;  // set when form window is done initializing

		private SearchHelpForm helpForm;

		public SearchForm()
		{
			bWindowInitComplete = false;  // we aren't done initializing the window yet, don't overwrite any .config settings

			this.DialogResult = DialogResult.Cancel;  // set the default dialog result to 'Cancel'

			helpForm = new SearchHelpForm();

			InitializeComponent();
		}

		private void SearchForm_Load(object sender, EventArgs e)
		{
			bool bConfigRegularExpression = false;
			if( Config.Get(Config.KEY.SearchRegularExpression, ref bConfigRegularExpression) )
			{
				RegExCheckBox.Checked = bConfigRegularExpression;
			}

			bool bConfigMatchCase = false;
			if( Config.Get(Config.KEY.SearchMatchCase, ref bConfigMatchCase) )
			{
				MatchCaseCheckBox.Checked = bConfigMatchCase;
			}

			bool bConfigRecursive = false;
			if( Config.Get(Config.KEY.SearchRecursive, ref bConfigRecursive) )
			{
				RecurseFolderCheckBox.Checked = bConfigRecursive;
			}

			List<string> SearchForList = new List<string>();
			if( Config.Get(Config.KEY.SearchText, ref SearchForList) )
			{
				for( int i = 0; i < SearchForList.Count; i++ )
				{
					int length = SearchForList[i].Length;
					if( (SearchForList[i].Substring(0,1) == "\"") || (SearchForList[i].Substring(length-1,1) == "\"") )
					{
						string value = SearchForList[i].Substring(1, length-2);  // chop off the leading and trailing double quote
						SearchForComboBox.Items.Add(value);

						if( i == 0 )  // automatically populate the search text box with the most recent config setting
						{
							SearchForComboBox.Text = value;
						}
					}
				}
			}

			List<string> FileSpecList = new List<string>();
			if( Config.Get(Config.KEY.SearchFileSpec, ref FileSpecList) )
			{
				for( int i = 0; i < FileSpecList.Count; i++ )
				{
					FileSpecComboBox.Items.Add(FileSpecList[i]);

					if( i == 0 )  // automatically populate the file spec box with the most recent config setting
					{
						FileSpecComboBox.Text = FileSpecList[i];
					}
				}
			}

			List<string> FolderList = new List<string>();
			if( Config.Get(Config.KEY.SearchFolder, ref FolderList) )
			{
				for( int i = 0; i < FolderList.Count; i++ )
				{
					FolderComboBox.Items.Add(FolderList[i]);

					if( i == 0 )  // automatically populate the search folder box with the most recent config setting (or with the commandline search folder)
					{
						FolderComboBox.Text = FolderList[i];
					}
				}
			}

			// if we have specified a search folder as the only argument (like when right clicking on a folder)...
			if( Globals.SearchDirectory != "" )
			{
				FolderComboBox.Text = Globals.SearchDirectory;  // populate the search folder box with that folder
			}

			ToolTip SearchToolTip = new ToolTip();

			SearchToolTip.AutomaticDelay = 1000;

			SearchToolTip.SetToolTip(this.RegExCheckBox, "Check to enable searching with regular expressions.");
			SearchToolTip.SetToolTip(this.MatchCaseCheckBox, "Check to enable searching with a case sensitive search.");
			SearchToolTip.SetToolTip(this.RecurseFolderCheckBox, "Check to search files in the specified folder and files in all sub-folders.");

			SearchToolTip.SetToolTip(this.SearchForComboBox, "Type in the text you wish to search for here.");
			SearchToolTip.SetToolTip(this.FileSpecComboBox, "Type in the file specifications you wish to use when collecting the files to search.");
			SearchToolTip.SetToolTip(this.FolderComboBox, "Type in the folder name you wish to start searching from (or click the \"...\" button to browse to a folder).");
		}

		private void SearchForm_Shown(object sender, EventArgs e)
		{
			int pos_x = -1;
			int pos_y = -1;
			if( Config.Get(Config.KEY.SearchPosX, ref pos_x) && Config.Get(Config.KEY.SearchPosY, ref pos_y) )
			{
				Location = new Point(pos_x, pos_y);
			}
			else  // otherwise, center the window on the main form
			{
				this.CenterToParent();
			}

			bWindowInitComplete = true;  // window initialization is complete, okay to write config settings now
		}

		private void SearchForm_Closing(object sender, FormClosingEventArgs e)
		{
			helpForm.Close();
		}

		private void SearchForm_Move(object sender, EventArgs e)
		{
			if( bWindowInitComplete )
			{
				Config.Set(Config.KEY.SearchPosX, Location.X);
				Config.Set(Config.KEY.SearchPosY, Location.Y);
			}
		}

		private void SaveConfig()
		{
			Config.Set(Config.KEY.SearchRegularExpression, RegExCheckBox.Checked);
			Config.Set(Config.KEY.SearchMatchCase, MatchCaseCheckBox.Checked);
			Config.Set(Config.KEY.SearchRecursive, RecurseFolderCheckBox.Checked);

			// get the list of search text items from the combo box
			List<string> SearchForComboBoxList = SearchForComboBox.Items.Cast<string>().ToList();

			// if the combo box SearchFor text is already in that list, remove it from the list
			int searchForIndex = SearchForComboBoxList.IndexOf(SearchFor);
			if( searchForIndex != -1 )
			{
				SearchForComboBoxList.RemoveAt(searchForIndex);  // remove duplicate
			}

			// if the combo box SearchFor text is already at the maximum number of elements, remove the last one
			if( SearchForComboBoxList.Count >= NUM_SEARCH_HISTORY )
			{
				SearchForComboBoxList.RemoveAt(NUM_SEARCH_HISTORY - 1);  // prune the list
			}

			// add the new SearchFor text to the start of the combo box list
			SearchForComboBoxList.Insert(0, SearchFor);

			// add the double quotes around the search text list
			List<string> SearchTextList = new List<string>();

			for( int i = 0; i < SearchForComboBoxList.Count; i++ )
			{
				SearchTextList.Add("\"" + SearchForComboBoxList[i] + "\"");
			}

			// save the search text list to the config file
			Config.Set(Config.KEY.SearchText, SearchTextList);


			// do the same for the FileSpec combo box
			List<string> FileSpecComboBoxList = FileSpecComboBox.Items.Cast<string>().ToList();
			int fileSpecIndex = FileSpecComboBoxList.IndexOf(FileSpec);
			if( fileSpecIndex != -1 )
			{
				FileSpecComboBoxList.RemoveAt(fileSpecIndex);  // remove duplicate
			}
			if( FileSpecComboBoxList.Count >= NUM_FILESPEC_HISTORY )
			{
				FileSpecComboBoxList.RemoveAt(NUM_FILESPEC_HISTORY - 1);  // prune the list
			}
			FileSpecComboBoxList.Insert(0, FileSpec);
			// we don't need to add double quotes to file specs
			Config.Set(Config.KEY.SearchFileSpec, FileSpecComboBoxList);


			// do the same for the Folder combo box
			List<string> FolderComboBoxList = FolderComboBox.Items.Cast<string>().ToList();
			int folderIndex = FolderComboBoxList.IndexOf(FolderName);
			if( folderIndex != -1 )
			{
				FolderComboBoxList.RemoveAt(folderIndex);  // remove duplicate
			}
			if( FolderComboBoxList.Count >= NUM_FOLDER_HISTORY )
			{
				FolderComboBoxList.RemoveAt(NUM_FOLDER_HISTORY - 1);  // prune the list
			}
			FolderComboBoxList.Insert(0, FolderName);
			// we don't need to add double quotes to folder names
			Config.Set(Config.KEY.SearchFolder, FolderComboBoxList);
		}

		private void ValidateInput()
		{
			if( SearchFor == "" )  // make sure text to search for has been provided
			{
				MessageBox.Show("'Search For' is empty.  You must provide text to search for.\n", "Invalid Search String");
				return;
			}

			if( FileSpecComboBox.Text == "" )  // make sure a file specification is provided
			{
				MessageBox.Show("'File Specification' is empty.  You must provide a file specification (or *.*).\n", "Invalid FileSpec");
				return;
			}

			if( FolderName == "" )  // make sure a folder to search has been provided
			{
				MessageBox.Show("'Folder To Search' is empty.  You must provide a folder to search.\n", "Invalid Folder To Search");
				return;
			}

			if( !Directory.Exists(FolderName) )
			{
				MessageBox.Show("'Folder To Search' directory does not exist.\n", "Invalid Directory");
				return;
			}

			if( FolderName.EndsWith("\\") )  // if folder name ends with a backslash, remove it
			{
				FolderComboBox.Text = FolderName.TrimEnd('\\');
			}

			if( RegExCheckBox.Checked )  // if using Regular Expression search, make sure the search text is a valid regular expression
			{
				try
				{
					Regex SearchRegEx = new Regex(SearchFor, RegexOptions.IgnoreCase);
				}
				catch
				{
					MessageBox.Show("The 'Search For' string is not a valid Regular Expression (See 'Help').\n(Correct the regular expression or uncheck the 'Regular Expression' checkbox)", "Invalid Regular Expression");
					return;
				}
			}

			SaveConfig();

			this.DialogResult = DialogResult.OK;

			Close();
		}

		private void FoldersButton_Click(object sender, EventArgs e)
		{
			FolderSelectDialog FolderSelect = new FolderSelectDialog();

			if( (FolderName != "") && Directory.Exists(FolderName) )
			{
				FolderSelect.InitialDirectory = FolderName;
			}
			else
			{
				FolderSelect.InitialDirectory = "";
			}

			if( FolderSelect.ShowDialog() && (FolderSelect.FileName != "") )
			{
				FolderComboBox.Text = FolderSelect.FileName;
			}
		}

		private void SearchOkButton_Click(object sender, EventArgs e)
		{
			ValidateInput();
		}

		private void SearchHelpButton_Click(object sender, EventArgs e)
		{
			helpForm.Show(this);
		}

		private void SearchCancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void SearchForComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Enter )
			{
				ValidateInput();
			}
		}

		private void FileSpecComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Enter )
			{
				ValidateInput();
			}
		}

		private void FolderComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Enter )
			{
				ValidateInput();
			}
		}
	}
}
