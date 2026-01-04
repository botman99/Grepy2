using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Timers;
using System.Diagnostics;

//
// NOTE: The app version number is in Properties\AssemblyInfo.cs
//

//
// HOTE: The HTML Help Workshop and documentation can be found here:
//		https://www.microsoft.com/en-us/download/details.aspx?id=21138
//		(Use it to open the .hhp file for easier help file editing.)
//

namespace Grepy2
{
	public partial class MainForm : Form
	{
		const int MAX_PATH = 260;

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct SHFILEINFO
		{
			public SHFILEINFO(bool b)
			{
				hIcon=IntPtr.Zero;
				iIcon=0;
				dwAttributes=0;
				szDisplayName="";
				szTypeName="";
			}

			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=80)]
			public string szTypeName;
		};

		const uint FILE_ATTRIBUTE_NORMAL = 0x80;
		const uint SHGFI_TYPENAME = 0x000000400;
		const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

		[DllImport("shell32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

	    [DllImport("user32.dll")]
	    public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int GetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar);


		private const int WM_SETREDRAW		= 0x000B;

		private const int EM_GETEVENTMASK	= (Globals.WM_USER + 59);  // from Richedit.h
		private const int EM_SETEVENTMASK	= (Globals.WM_USER + 69);  // from Richedit.h

		private const int WM_HSCROLL		= 0x0114;  // from WinUser.h
		private const int WM_VSCROLL		= 0x0115;  // from WinUser.h

		private const int SB_THUMBPOSITION	= 4;  // from WinUser.h


		struct FilenameIndex_s  // keeps track of which line in the RichTextBox is the filename line (used when user wants to open the file)
		{
			public int RichTextLineIndex;
			public int FilenameIndex;
		}


		private Thread[] WorkerThreads;
		private Thread GetFilesThread;

		private bool bWindowInitComplete;  // set when main form window is done initializing
		private bool bShouldSaveMoveLocation = false;  // set when this main window is currently minimizing, maximizing or restoring
		private bool bShouldSaveResizeSize = false;  // set when this main window is currently minimizing, maximizing or restoring

		private CollectingFiles collectingFilesDialog = null;  // dialog displayed while the GetFiles thread is collecting the files to search
		private PleaseWait pleaseWaitDialog = null;  // dialog displayed when redrawing the RichText box with more than 1000 lines

		private System.Timers.Timer CollectFilesTimer;
		private System.Timers.Timer SliderUpdateTimer;

		private bool bIsSearchInProgress = false;
		private int NextSearchFileJobIndex = 0;  // used to track which SearchFile is next to be assigned to worker threads

		private int LowestWorkerFileProcessedIndex = 0;  // used to keep track of the lowest SearchFile index that has been processed by a worker

		private ListViewColumnSorter columnSorter;
		private bool bDisableListViewSorting = false;

		private int TotalNumberOfSearchMatches = 0;
		private int TotalNumberOfSearchedFiles = 0;
		private int TotalNumberOfSearchFileLines = 0;  // used to track the total number of lines that potentially could be displayed in the rich text box

		private int PleaseWaitDeferredIndex = 0;
		private bool PleaseWaitForceDisplay = false;

		private bool bWasPreviousRichTextDisplayCancelled = false;

		private int[] ColumnWidthSize;  // store the ListView column width size so we can tell if they have actually changed

		private int ListViewFileClickedOnIndex = -1;  // this indicates if we have clicked on a file from the ListView to display the text for just that file

		// when switching from a specific file to showing all files in the RichTextBox, we want to "syncronize" the position so that we adjust the cursor
		// position after re-displaying all files such that the file that was displayed (the specific file) is still displayed (as though it hadn't moved)
		private int RichTextSyncFileIndex = -1;  // file index to syncronize with
		private int RichTextSyncCharacterIndex = -1;  // RichTextBox character index of line we want to syncronize to
		private int RichTextCaretCharacterIndex = -1;  // RichText character index to sync to after re-displaying all files

		private List<int> RichTextMatchSyncList;

		private List<FilenameIndex_s> FilenameLineNumbers;  // so we can determine the filename based on the RichText line number

		private int[] TabPositions;

		private ListViewItem FileListViewSelectedItem;
		private Color FileListViewBackColor;

		private bool bDeferRichTextDisplay;
		private bool bUseWindowsFileAssociation;
		private string CustomEditorString;

		private static bool bIsInWorkerNotify = false;
		private static int WorkerNotifyIndex = 0;
		private static List<int> ResendMessageList = null;


		public MainForm()
		{
			bWindowInitComplete = false;  // we aren't done initializing the window yet, don't overwrite any .config settings

			InitializeComponent();

			// load config file settings and set window position, window size, maximized setting, splitter positions, etc.
			int pos_x = -1;
			int pos_y = -1;
			if( Config.Get(Config.KEY.PosX, ref pos_x) && Config.Get(Config.KEY.PosY, ref pos_y) )
			{
				this.StartPosition = FormStartPosition.Manual;
				Location = new Point(pos_x, pos_y);
			}

			int size_x = -1;
			int size_y = -1;
			if( Config.Get(Config.KEY.Width, ref size_x) && Config.Get(Config.KEY.Height, ref size_y) )
			{
				Size = new System.Drawing.Size(size_x, size_y);
			}

			bool bMaximized = false;
			if( Config.Get(Config.KEY.Maximized, ref bMaximized) )
			{
				this.WindowState = bMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
			}
		}

		protected override void WndProc(ref Message m)
		{
			const int WM_SYSCOMMAND = 0x0112;

			switch( m.Msg )
			{
				case WM_SYSCOMMAND:
				{
					if( (m.WParam.ToInt32() & 0xFFF0) == 0xF020 ) // Minimize event - SC_MINIMIZE from Winuser.h
					{
						bShouldSaveMoveLocation = false;
						bShouldSaveResizeSize = false;
					}
					if( (m.WParam.ToInt32() & 0xFFF0) == 0xF030 ) // Maximize event - SC_MAXIMIZE from Winuser.h
					{
						bShouldSaveMoveLocation = false;
						bShouldSaveResizeSize = false;
					}
					else if( (m.WParam.ToInt32() & 0xFFF0) == 0xF120 ) // Restore event - SC_RESTORE from Winuser.h
					{
						bShouldSaveMoveLocation = false;
						bShouldSaveResizeSize = false;
					}
				}
				break;

				case Globals.WM_GETFILES_NOTIFY_MAIN_THREAD:
				{
					if( collectingFilesDialog != null )
					{
						collectingFilesDialog.Close();
						collectingFilesDialog = null;
					}

					if( !Globals.GetFiles.bShouldStopCurrentJob )  // if we are done collecting files (and didn't cancel collecting files), then start searching those files...
					{
						SearchingLabel.Enabled = true;
						SearchingProgressBar.Value = 0;

						string search_count_string = "";
						if( Globals.SearchDirectoryCount == 1 )
						{
							search_count_string = "1 Folder,  ";  // 2 spaces for better separation
						}
						else
						{
							search_count_string = string.Format("{0} Folders,  ", Globals.SearchDirectoryCount.ToString("N0"));  // 2 spaces for better separation
						}

						if( Globals.SearchFiles.Length == 1 )
						{
							search_count_string += "1 File";
						}
						else
						{
							search_count_string += string.Format("{0} Files", Globals.SearchFiles.Length.ToString("N0"));
						}

						SearchCountLabel.Text = search_count_string;
						SearchCountLabel.Enabled = true;
						SearchCountLabel.Visible = true;

						if( Globals.SearchFiles.Length > 0 )  // if any files were collected, start searching them...
						{
							stopSearchToolStripMenuItem.Visible = true;
							stopSearchToolStripMenuItem.Enabled = true;

							string SearchForFileSpec = Globals.FileSpecs[0];
							for( int index = 1; index < Globals.FileSpecs.Count; index++ )
							{
								SearchForFileSpec = SearchForFileSpec + " " + Globals.FileSpecs[index];
							}

							string SearchStatusText = string.Format("Searching {0} for: {1}", SearchForFileSpec, Globals.SearchString);
							searchStatusLabel.Text = SearchStatusText;

							bIsSearchInProgress = true;

							RichTextMatchSyncList = new List<int>();
							FilenameLineNumbers = new List<FilenameIndex_s>();

							NextSearchFileJobIndex = 0;
							LowestWorkerFileProcessedIndex = 0;

							int NumWorkersToStart = Math.Min(Globals.SearchFiles.Length, Globals.Workers.Length);

							Globals.bShouldStopWorkerJobs = false;

							Globals.NumFilesSearched = 0;

							for( int i = 0; i < NumWorkersToStart; i++ )
							{
								Globals.Workers[i].SearchFilesIndex = NextSearchFileJobIndex;
								NextSearchFileJobIndex++;

								Globals.Workers[i].WaitHandle.Set();  // tell the Worker thread that to search the SearchFiles index item
							}
						}
						else  // otherwise, turn off the "Searching:" label
						{
							SearchingLabel.Enabled = false;
							SearchingProgressBar.Value = 0;

							string SearchForFileSpec = Globals.FileSpecs[0];
							for( int index = 1; index < Globals.FileSpecs.Count; index++ )
							{
								SearchForFileSpec = SearchForFileSpec + " " + Globals.FileSpecs[index];
							}

							SearchIsDoneOrCancelled(true);
						}
					}
					else  // collecting the files to be searched was cancelled
					{
						SearchingLabel.Enabled = false;
						SearchingProgressBar.Value = 0;

						searchStatusLabel.Text = "";

						SearchIsDoneOrCancelled(false);
					}
				}
				break;

				case Globals.WM_WORKER_NOTIFY_MAIN_THREAD:
				{
					// we can get notify messages from worker threads while we are processing a notify message from a
					// worker thread due to the fact that we use Application.DoEvents() to keep pumping
					// Windows messages while displaying RichText.  We need to prevent this code from being reentrant.
					if( bIsInWorkerNotify )
					{
						// if we get a notify message from a worker we are already processing, ignore it
						if( (int)m.WParam == WorkerNotifyIndex )
						{
							return;
						}

						// otherwise, queue up the message so we can resend it
						ResendMessageList.Add((int)m.WParam);

						return;
					}

					bIsInWorkerNotify = true;

					WorkerNotifyIndex = (int)m.WParam;

					if( bIsSearchInProgress )
					{
						Globals.NumFilesSearched++;

						SearchingProgressBar.Value = (Globals.NumFilesSearched * 100) / Globals.SearchFiles.Length;

						// We want to add the files to the ListView in the sorted order that they appear in the SearchFiles array.
						// Workers will complete files at different rates, so we add files to the ListView once that file's index
						// in the SearchFiles array has been searched and we add as many as we can in each update.
						while( LowestWorkerFileProcessedIndex < Globals.SearchFiles.Length )
						{
							if( Globals.SearchFiles[LowestWorkerFileProcessedIndex] != null )
							{
								if( Globals.SearchFiles[LowestWorkerFileProcessedIndex].Status == Globals.SearchFileStatus.NotProcessed )
								{
									break;  // if we reach a file that has not been processed yet, bail out of the loop
								}

								// if the search text was found in this file, add it to the ListView otherwise we just skip over files that don't contain the search text
								if( Globals.SearchFiles[LowestWorkerFileProcessedIndex].Status == Globals.SearchFileStatus.SearchTextFound )
								{
									// get the file information for the file (filetype, filesize, modification time, etc.)
									SHFILEINFO info = new SHFILEINFO();
									SHGetFileInfo(Globals.SearchFiles[LowestWorkerFileProcessedIndex].Filename, FILE_ATTRIBUTE_NORMAL, ref info, (uint)Marshal.SizeOf(info), SHGFI_TYPENAME | SHGFI_USEFILEATTRIBUTES);

									Globals.SearchFiles[LowestWorkerFileProcessedIndex].FileTypeName = info.szTypeName;

									FileInfo fInfo = new FileInfo(Globals.SearchFiles[LowestWorkerFileProcessedIndex].Filename);

									Globals.SearchFiles[LowestWorkerFileProcessedIndex].FileLength = fInfo.Length;
									Globals.SearchFiles[LowestWorkerFileProcessedIndex].FileLastWriteTime = fInfo.LastWriteTime;


									ListViewItem item = CreateListViewItem(ref Globals.SearchFiles[LowestWorkerFileProcessedIndex], LowestWorkerFileProcessedIndex);

									FileListView.Items.Add(item);

									// if we aren't displaying an individual file from the ListView and we are not deferring RichText display while we search...
									if( (ListViewFileClickedOnIndex == -1) && !bDeferRichTextDisplay )
									{
										// display the RichText search match text for this file
										DisplayFileAsRichText(LowestWorkerFileProcessedIndex);
									}
								}
							}

							LowestWorkerFileProcessedIndex++;
						}

						if( Globals.NumFilesSearched == Globals.SearchFiles.Length )
						{
							// we are done searching through files
							stopSearchToolStripMenuItem.Visible = false;
							stopSearchToolStripMenuItem.Enabled = false;

							SearchIsDoneOrCancelled(true);

							if( bDeferRichTextDisplay )
							{
								DeferReDisplayRichTextSearchMatches(-1);
							}

							CollectFilesTimer.Interval = 500;  // expire in 1/2 second
							CollectFilesTimer.Enabled = true;
						}
						else
						{
							if( Globals.bShouldStopWorkerJobs )  // if we have cancelled the search...
							{
								bool bAllWorkerThreadsStopped = true;  // assume true until known otherwise

								for( int i = 0; i < Globals.Workers.Length; i++ )
								{
									if( Globals.Workers[i].bIsWorking )  // if a worker thread is still working then we aren't done yet
									{
										bAllWorkerThreadsStopped = false;
										break;
									}
								}

								if( bAllWorkerThreadsStopped )  // all the worker threads have stopped searching the files
								{
									SearchingLabel.Enabled = false;
									SearchingProgressBar.Value = 0;

									SearchIsDoneOrCancelled(true);

									Globals.bShouldStopWorkerJobs = false;

									if( bDeferRichTextDisplay )
									{
										DeferReDisplayRichTextSearchMatches(-1);
									}
								}
							}
							else if( NextSearchFileJobIndex < Globals.SearchFiles.Length )
							{

								Globals.Workers[WorkerNotifyIndex].SearchFilesIndex = NextSearchFileJobIndex;
								NextSearchFileJobIndex++;

								Globals.Workers[WorkerNotifyIndex].WaitHandle.Set();  // tell the Worker thread that to search the SearchFiles index item
							}
						}
					}

					if( (ResendMessageList != null) && (ResendMessageList.Count > 0) )
					{
						HandleRef FormHandle = new HandleRef(this, Globals.MainFormHandle);

						for( int i = 0; i < ResendMessageList.Count; i++ )
						{
							IntPtr IndexPtr = new IntPtr(ResendMessageList[i]);
							PostMessage(FormHandle, Globals.WM_WORKER_NOTIFY_MAIN_THREAD, IndexPtr, IntPtr.Zero);
						}

						ResendMessageList.Clear();
					}

					bIsInWorkerNotify = false;
				}
				break;

				case Globals.WM_SLIDERUPDATE_NOTIFY_MAIN_THREAD:
				{
					// if there are more than 1,000 lines in the rich text box, then updating it will take some time, display a "Please Wait" dialog
					if( TotalNumberOfSearchMatches > 1000 )
					{
						string msg = string.Format("{0} matches were found.\nThis could take a long time to display.\nAre you sure you wish to display all of the results?", TotalNumberOfSearchMatches);

						if( TotalNumberOfSearchMatches > 5000 )
						{
							msg = string.Format("{0} matches were found.\nThis could take a REALLY long time (minutes) to display.\nAre you sure you wish to display all of the results?", TotalNumberOfSearchMatches);
						}

						DialogResult result = MessageBox.Show(msg, "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

						if( result == DialogResult.No )
						{
							break;
						}
					}

					if( TotalNumberOfSearchFileLines > 1000 )
					{
						PleaseWaitDeferredIndex = ListViewFileClickedOnIndex;
						PleaseWaitForceDisplay = false;

						pleaseWaitDialog = new PleaseWait();

						pleaseWaitDialog.StartPosition = FormStartPosition.CenterParent;
						pleaseWaitDialog.ControlBox = false;  // don't show the 'X' to close button

						pleaseWaitDialog.ShowDialog();
					}
					else
					{
						// update the before/after preview on whatever is currently displayed
						ReDisplayRichTextSearchMatches(ListViewFileClickedOnIndex);
					}
				}
				break;

				case Globals.WM_PLEASEWAITSHOWN_NOTIFY_MAIN_THREAD:
				{
					// update the before/after preview on whatever is currently displayed
					ReDisplayRichTextSearchMatches(PleaseWaitDeferredIndex, PleaseWaitForceDisplay);

					if( pleaseWaitDialog != null )
					{
						pleaseWaitDialog.Close();
						pleaseWaitDialog = null;
					}
				}
				break;

				case Globals.WM_CTRL_CLICK_NOTIFY_MAIN_THREAD:
				{
					OpenFileFromRichTextBoxPoint(RichTextBox.RichTextMousePosition);

					FileListView.Focus();  // set focus on the FileListView to keep any selected items selected (since clicking in the RichTextBox will change the focus)
				}
				break;
			}

			base.WndProc(ref m);
		}

		private void SearchIsDoneOrCancelled(bool bDisplaySearchTextMatches)
		{
			// enable all things that were disabled while a search was in progress...

			searchCtrlFToolStripMenuItem.Enabled = true;
			optionsToolStripMenuItem.Enabled = true;

			allSearchMatchesToolStripMenuItem.Enabled = true;

			bIsSearchInProgress = false;
			bDisableListViewSorting = false;

			columnSorter.SortColumn = 0;
			columnSorter.Order = SortOrder.None;
			columnSorter.SetSortIcon(FileListView);

			FileListView.ListViewItemSorter = columnSorter;  // enable the ListView column sorting

			// count the number of lines (including preview lines) that were found for files with matches
			TotalNumberOfSearchFileLines = 0;
			for( int index = 0; index < Globals.SearchFiles.Length; index++ )
			{
				if( (Globals.SearchFiles[index] != null) && (Globals.SearchFiles[index].Status == Globals.SearchFileStatus.SearchTextFound) )
				{
					TotalNumberOfSearchFileLines += Globals.SearchFiles[index].Lines.Count;
				}
			}

			if( bDisplaySearchTextMatches )
			{
				string SearchForFileSpec = Globals.FileSpecs[0];
				for( int index = 1; index < Globals.FileSpecs.Count; index++ )
				{
					SearchForFileSpec = SearchForFileSpec + " " + Globals.FileSpecs[index];
				}

				TotalNumberOfSearchMatches = 0;
				TotalNumberOfSearchedFiles = 0;
				for( int index = 0; index < Globals.SearchFiles.Length; index++ )
				{
					if( Globals.SearchFiles[index] != null )
					{
						TotalNumberOfSearchMatches += Globals.SearchFiles[index].Matches;
						if( Globals.SearchFiles[index].Status == Globals.SearchFileStatus.SearchTextFound )
						{
							TotalNumberOfSearchedFiles++;
						}
					}
				}

				if( Globals.SearchFiles.Length == 0 )
				{
					searchStatusLabel.Text = string.Format("No files found matching file specification: {0}", SearchForFileSpec);
				}
				else
				{
					searchStatusLabel.Text = string.Format("Search for <{0}> in <{1}>: {2} {3} in {4} {5}", Globals.SearchString, SearchForFileSpec, TotalNumberOfSearchMatches, (TotalNumberOfSearchMatches == 1) ? "match" : "matches", TotalNumberOfSearchedFiles, (TotalNumberOfSearchedFiles == 1) ? "file" : "files");
				}

				if( TotalNumberOfSearchMatches == 0 )
				{
					NoMatchesFound noMatchesFoundDialog = new NoMatchesFound();

					noMatchesFoundDialog.StartPosition = FormStartPosition.CenterParent;
//					noMatchesFoundDialog.ControlBox = false;  // don't show the 'X' to close button

					noMatchesFoundDialog.ShowDialog(this);
				}
			}
		}

		private ListViewItem CreateListViewItem(ref Globals.SearchFile SearchFile, int index)
		{
			// create a ListViewItem with the search file base filename
			ListViewItem item = new ListViewItem(SearchFile.BaseFilename);

			item.SubItems.Add(SearchFile.FileTypeName); // add the file type (column 2)
			item.SubItems.Add(SearchFile.FolderName);  // add the folder name (column 3)
			item.SubItems.Add(SearchFile.SearchMatchCount.ToString());  // add the number of search matches in the file (colume 4)
			item.SubItems.Add(SearchFile.FileLength.ToString());  // add the file length (column 5)

			item.SubItems.Add(string.Format("{0,4:D4}/{1,2:D2}/{2,2:D2} {3,2:D2}:{4,2:D2}:{5,2:D2}",  // add the file modified time (colum 6)
				SearchFile.FileLastWriteTime.Year, SearchFile.FileLastWriteTime.Month, SearchFile.FileLastWriteTime.Day,
				SearchFile.FileLastWriteTime.Hour, SearchFile.FileLastWriteTime.Minute, SearchFile.FileLastWriteTime.Second));

			// add a "hidden" column with the index of the original 'SearchFiles' item (so we know the original index after columns are sorted)
			item.SubItems.Add( System.Convert.ToString(index) );

			return item;
		}

		private void AddLineToRichTextBoxTextBuffer(int FileIndex, int LineIndex, ref string text_buffer, ref int PreviousLineNumber)
		{
			if( (PreviousLineNumber > 0) && (PreviousLineNumber < Globals.SearchFiles[FileIndex].Lines[LineIndex].LineNumber - 1) )
			{
				text_buffer += "\n";  // break between search match gaps
			}

			if (displayLineNumbersToolStripMenuItem.Checked)
			{
				text_buffer += string.Format("{0,8}: ", Globals.SearchFiles[FileIndex].Lines[LineIndex].LineNumber);
			}

			if( Globals.SearchFiles[FileIndex].Lines[LineIndex].bIsSearchTextMatch )
			{
				RichTextBox.AppendText(text_buffer);

				if( RichTextMatchSyncList != null )
				{
					RichTextMatchSyncList.Add(RichTextBox.Lines.Length - 1);  // minus one here because we've already appended the line number for this match line in the code above
				}

				int output_pos = 0;
				for( int index = 0; index < Globals.SearchFiles[FileIndex].Lines[LineIndex].SearchMatchPositions.Count; index++ )
				{
					int search_match_start_pos = Globals.SearchFiles[FileIndex].Lines[LineIndex].SearchMatchPositions[index].StartPos;
					int search_match_length = Globals.SearchFiles[FileIndex].Lines[LineIndex].SearchMatchPositions[index].Length;
					if( output_pos < search_match_start_pos )
					{
						RichTextBox.AppendText(Globals.SearchFiles[FileIndex].Lines[LineIndex].Line.Substring(output_pos, search_match_start_pos - output_pos));
					}

					RichTextBox.SelectionColor = Color.Red;
					RichTextBox.AppendText(Globals.SearchFiles[FileIndex].Lines[LineIndex].Line.Substring(search_match_start_pos, search_match_length));
					RichTextBox.SelectionColor = MainForm.DefaultForeColor;

					output_pos = search_match_start_pos + search_match_length;
				}

				text_buffer = "";

				int line_length = Globals.SearchFiles[FileIndex].Lines[LineIndex].Line.Length;
				if( output_pos < line_length )
				{
					text_buffer += Globals.SearchFiles[FileIndex].Lines[LineIndex].Line.Substring(output_pos, line_length - output_pos);
				}
			}
			else
			{
				text_buffer += Globals.SearchFiles[FileIndex].Lines[LineIndex].Line;
			}

			text_buffer += "\n";

			PreviousLineNumber = Globals.SearchFiles[FileIndex].Lines[LineIndex].LineNumber;
		}

		private void DisplayFileAsRichText(int FileIndex)
		{
			if( Globals.SearchFiles[FileIndex] == null )
			{
				return;
			}

			if( RichTextBox.TextLength != 0 )  // if there's already some text in the RichTextBox...
			{
				RichTextBox.AppendText("\n");  // add blank line before the filename
			}

			if( FileIndex == RichTextSyncFileIndex )  // if the current file is the one we want to syncronize with...
			{
				RichTextCaretCharacterIndex = RichTextBox.TextLength + RichTextSyncCharacterIndex;  // store the current character index plus the offset that we calculated earlier
			}

			if( FilenameLineNumbers != null )
			{
				FilenameIndex_s FilenameIndex = new FilenameIndex_s();
				FilenameIndex.RichTextLineIndex = RichTextBox.Lines.Length;  // this is the RichText line number where we are about to display the filename
				FilenameIndex.FilenameIndex = FileIndex;

				FilenameLineNumbers.Add(FilenameIndex);
			}

			// display the filename (in bold or non-bold text depending on the RichTextBox font)
			bool SelectionFontIsBold = RichTextBox.SelectionFont.Bold;
			RichTextBox.SelectionFont = new Font(RichTextBox.SelectionFont, SelectionFontIsBold ? FontStyle.Regular : FontStyle.Bold);
			RichTextBox.SelectedText = string.Format("{0}\\{1}\n", Globals.SearchFiles[FileIndex].FolderName, Globals.SearchFiles[FileIndex].BaseFilename);
			RichTextBox.SelectionFont = new Font(RichTextBox.SelectionFont, SelectionFontIsBold ? FontStyle.Bold : FontStyle.Regular);

			RichTextBox.AppendText("\n");  // add blank line after the filename

			bool bAfterDisplayedMatch = false;  // tracks whether we are displaying preview text after a match
			int PreviousLineNumber = 0;  // used to detect when we need to output a blank line between sets of preview lines
			int LinesAfterMatch = 0;  // how many lines we've output after a search text match line was output

			string text_buffer = "";  // buffer up all the lines of text that will be displayed in the RichTextBox and Append them all at once (for maximum speed)

			bool bCancelDisplayOfText = false;

			for( int LineIndex = 0; !bCancelDisplayOfText && (LineIndex < Globals.SearchFiles[FileIndex].Lines.Count); LineIndex++ )
			{
				Application.DoEvents();  // keep pumping the main thread message queue (so we can cancel things by pressing buttons)

				if( (pleaseWaitDialog != null) && Globals.bPleaseWaitDialogCancelled )
				{
					bCancelDisplayOfText = true;
				}

				if( Globals.SearchFiles[FileIndex].Lines[LineIndex].bIsSearchTextMatch )  // does this line have a search text match? (we always output search text match lines)
				{
					AddLineToRichTextBoxTextBuffer(FileIndex, LineIndex, ref text_buffer, ref PreviousLineNumber);

					bAfterDisplayedMatch = true;
					LinesAfterMatch = 0;
				}
				else
				{
					// otherwise, if this line is within the preview range before a match output it (this might be before or after a match)
					if( (Globals.SearchFiles[FileIndex].Lines[LineIndex].LinesBeforeNextMatch > 0 ) &&
						(Globals.SearchFiles[FileIndex].Lines[LineIndex].LinesBeforeNextMatch <= PreviewBeforeTrackBar.Value) )
					{
						AddLineToRichTextBoxTextBuffer(FileIndex, LineIndex, ref text_buffer, ref PreviousLineNumber);
					}
					// otherwise, if we are after a match has been displayed and the lines output are within the preview 'after' range...
					else if( bAfterDisplayedMatch && (++LinesAfterMatch <= PreviewAfterTrackBar.Value) )
					{
						AddLineToRichTextBoxTextBuffer(FileIndex, LineIndex, ref text_buffer, ref PreviousLineNumber);
					}
				}
			}

			RichTextBox.AppendText(text_buffer);
		}

		private void DeferReDisplayRichTextSearchMatches(int IndexToDisplay)
		{
			if( TotalNumberOfSearchMatches > 1000 )
			{
				string msg = string.Format("{0} matches were found.\nThis could take a long time to display.\nAre you sure you wish to display all of the results?", TotalNumberOfSearchMatches);

				if( TotalNumberOfSearchMatches > 5000 )
				{
					msg = string.Format("{0} matches were found.\nThis could take a REALLY long time (minutes) to display.\nAre you sure you wish to display all of the results?", TotalNumberOfSearchMatches);
				}

				DialogResult result = MessageBox.Show(msg, "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

				if( result == DialogResult.No )
				{
					return;
				}
			}

			if( TotalNumberOfSearchFileLines > 1000 )
			{
				PleaseWaitDeferredIndex = IndexToDisplay;
				PleaseWaitForceDisplay = true;

				pleaseWaitDialog = new PleaseWait();

				pleaseWaitDialog.StartPosition = FormStartPosition.CenterParent;
				pleaseWaitDialog.ControlBox = false;  // don't show the 'X' to close button

				pleaseWaitDialog.ShowDialog();
			}
			else
			{
				ReDisplayRichTextSearchMatches(IndexToDisplay, true);
			}
		}

		private void ReDisplayRichTextSearchMatches(int IndexToDisplay, bool bForceDisplayOfAllFiles = false)
		{
			if( Globals.SearchFiles == null )
			{
				return;
			}

			bool bSyncronizeUsingMatchLine = false;
			int MatchSyncLineIndex = -1;
			int MatchSyncLineOffset = 0;

			// if we are going from displaying all files to a single file or from displaying a single file to displaying all files...
			if( bForceDisplayOfAllFiles || (ListViewFileClickedOnIndex != IndexToDisplay) )  // only re-display the file(s) if something has changed
			{
				if( IndexToDisplay == -1 )  // if we are re-displaying all files, syncronize to the current position of the file currently being displayed
				{
					int FontHeight = RichTextBox.Font.Height;
					int HalfCharacterHeight = (int)(FontHeight / 2.0f);

					// get the RichTextBox character index of the first character of the topmost visible line
					RichTextSyncCharacterIndex = RichTextBox.GetCharIndexFromPosition(new Point(0,HalfCharacterHeight));

					RichTextSyncFileIndex = ListViewFileClickedOnIndex;  // store the index of the file we want to sync with

					RichTextCaretCharacterIndex = -1;  // this will be set by DisplayFileAsRichText() when we start re-displaying the current file
				}
				else
				{
					RichTextSyncFileIndex = -1;
					RichTextSyncCharacterIndex = -1;
					RichTextCaretCharacterIndex = -1;
				}
			}
			else  // otherwise we are adjusting the 'before' or 'after' preview slider, we will syncronize to the first displayed 'search text match' line
			{
				RichTextSyncFileIndex = -1;
				RichTextSyncCharacterIndex = -1;
				RichTextCaretCharacterIndex = -1;

				int FontHeight = RichTextBox.Font.Height;
				int HalfCharacterHeight = (int)(FontHeight / 2.0f);

				// get the RichTextBox character index of the first character of the topmost visible line
				int TopLineCharacterIndex = RichTextBox.GetCharIndexFromPosition(new Point(0,HalfCharacterHeight));
				int TopLineIndex = RichTextBox.GetLineFromCharIndex(TopLineCharacterIndex);

				// find the Match line index that's greater than (or equal to) the TopLine index
				for( int index = 0; (RichTextMatchSyncList != null) && (index < RichTextMatchSyncList.Count); index++ )
				{
					if( RichTextMatchSyncList[index] >= TopLineIndex )
					{
						// set this to the index of the Match line (this index will be the same after we re-display the file(s)
						// but the RichText.Lines array index will be different after re-displaying the lines)
						MatchSyncLineIndex = index;  // this tells us which "Match" line we want to synchronize with
						break;
					}
				}

				if (MatchSyncLineIndex >= 0)
				{
					MatchSyncLineOffset = TopLineIndex - RichTextMatchSyncList[MatchSyncLineIndex];
				}

				bSyncronizeUsingMatchLine = true;
			}

			ListViewFileClickedOnIndex = IndexToDisplay;

			// http://weblogs.asp.net/jdanforth/88458

			// Stop redrawing:
			SendMessage(RichTextBox.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

			// Stop sending of events:
			IntPtr eventMask = IntPtr.Zero;
			eventMask = SendMessage(RichTextBox.Handle, EM_GETEVENTMASK, IntPtr.Zero, IntPtr.Zero);

			RichTextBox.Clear();

			if ((TabPositions != null) && (TabPositions.Length > 0))
			{
				RichTextBox.SelectionTabs = TabPositions;
			}

			RichTextMatchSyncList = new List<int>();
			FilenameLineNumbers = new List<FilenameIndex_s>();

			bWasPreviousRichTextDisplayCancelled = false;

			if( ListViewFileClickedOnIndex != -1 )  // if we aren't re-displaying all files, display the file for the index
			{
				DisplayFileAsRichText(ListViewFileClickedOnIndex);

				if( !bSyncronizeUsingMatchLine )
				{
					RichTextBox.Select(0, 1);  // set the caret position to the very top of the RichText
					RichTextBox.ScrollToCaret();  // makes the selected line be the top line
					RichTextBox.DeselectAll();
				}
				else
				{
					if( (MatchSyncLineIndex != -1) && (RichTextMatchSyncList != null) && (MatchSyncLineIndex < RichTextMatchSyncList.Count) )
					{
						int TopSyncLine = Math.Max(0, RichTextMatchSyncList[MatchSyncLineIndex] + MatchSyncLineOffset);
						int CharIndex = RichTextBox.GetFirstCharIndexFromLine(TopSyncLine);
						RichTextBox.Select(CharIndex, 1);
						RichTextBox.ScrollToCaret();  // makes the selected line be the top line
						RichTextBox.DeselectAll();
					}
				}
			}
			else
			{
				for( int index = 0; index < Globals.SearchFiles.Length; index++ )
				{
					if( (pleaseWaitDialog != null) && Globals.bPleaseWaitDialogCancelled )
					{
						bWasPreviousRichTextDisplayCancelled = true;

						break;
					}

					if( Globals.SearchFiles[index].Status == Globals.SearchFileStatus.SearchTextFound )
					{
						DisplayFileAsRichText(index);
					}

					if( pleaseWaitDialog != null )
					{
						pleaseWaitDialog.UpdateProgressBar( (index * 100) / Globals.SearchFiles.Length );
					}
				}

				if( !bSyncronizeUsingMatchLine )
				{
					if( RichTextCaretCharacterIndex != -1 )  // if we have a character index position to syncronize with, set the caret there
					{
						RichTextBox.Select(RichTextCaretCharacterIndex, 1);
						RichTextBox.ScrollToCaret();  // makes the selected line be the top line
						RichTextBox.DeselectAll();
					}
				}
				else
				{
					if( (MatchSyncLineIndex != -1) && (RichTextMatchSyncList != null) && (MatchSyncLineIndex < RichTextMatchSyncList.Count) )
					{
						int TopSyncLine = Math.Max(0, RichTextMatchSyncList[MatchSyncLineIndex] + MatchSyncLineOffset);
						int CharIndex = RichTextBox.GetFirstCharIndexFromLine(TopSyncLine);
						RichTextBox.Select(CharIndex, 1);
						RichTextBox.ScrollToCaret();  // makes the selected line be the top line
						RichTextBox.DeselectAll();
					}
				}
			}

			// turn on events
			SendMessage(RichTextBox.Handle, EM_SETEVENTMASK, IntPtr.Zero, eventMask);

			// turn on redrawing
			int One = 1;
			IntPtr IntPtrOne = new IntPtr(One);
			SendMessage(RichTextBox.Handle, WM_SETREDRAW, IntPtrOne, IntPtr.Zero);

			RichTextBox.Refresh();
		}

		private void OpenFileFromRichTextBoxPoint(Point MousePosition)
		{
			if( FilenameLineNumbers == null )  // if we don't have a list of filename line numbers then nothing is displayed
			{
				return;
			}

			int CharIndex = RichTextBox.GetCharIndexFromPosition(MousePosition);
			int RichTextBoxLine = RichTextBox.GetLineFromCharIndex(CharIndex);

			if (RichTextBoxLine >= RichTextBox.Lines.Count())
			{
				RichTextBoxLine = RichTextBox.Lines.Count() - 2;  // minus 2 because the last line displayed will be blank
			}

			if( RichTextBox.Lines[RichTextBoxLine] == "" )  // empty lines can't be opened in editor because we don't know what line number it is
			{
				return;
			}

			// find the index of the file to open
			int FileToOpenIndex = -1;
			for( int index = 0; (index < FilenameLineNumbers.Count); index++ )
			{
				if( FilenameLineNumbers[index].RichTextLineIndex > RichTextBoxLine )  // are we done?
				{
					break;
				}

				FileToOpenIndex = FilenameLineNumbers[index].FilenameIndex;
			}

			if( FileToOpenIndex >= 0 )
			{
				int LineNumber = -1;
				Int32.TryParse(RichTextBox.Lines[RichTextBoxLine].Substring(0, 8), out LineNumber);

				if( (LineNumber > 0) && (Globals.SearchFiles[FileToOpenIndex] != null) )
				{
					OpenFile(Globals.SearchFiles[FileToOpenIndex].Filename, LineNumber);
				}
			}
		}

		private void OpenFile(string FilePath, int LineNumber)
		{
			if( !File.Exists(FilePath) )
			{
				string Msg = string.Format("The file '{0}' doesn't exist.", FilePath);
				MessageBox.Show(Msg, "File Doesn't Exist");
				return;
			}

			if( bUseWindowsFileAssociation )
			{
				try
				{
					// open editor here (using system default file association)
					ProcessStartInfo pi = new ProcessStartInfo(FilePath);
					pi.WorkingDirectory = Path.GetDirectoryName(FilePath);
					pi.UseShellExecute = true;
					pi.Verb = "OPEN";
					Process.Start(pi);
				}
				catch(Exception ex)
				{
					if( ex.Message == "Application not found" )
					{
						MessageBox.Show("Windows File Association is not set for this file type.", "Unassigned File Type");
					}
					else
					{
						MessageBox.Show("There was not executable found for this file type.\n(Windows File Association is not set or executable is missing.)", "Application not found");
					}
				}
			}
			else
			{
				string ProcString = "";
				string ArgString = "";

				string TempString = CustomEditorString;

				if( TempString.Substring(0, 1) == "\"" && (TempString.Length > 1) )
				{
					int index = TempString.IndexOf('"', 1);

					if( index > 0 )
					{
						ProcString = TempString.Substring(1, index - 1);

						TempString = TempString.Substring(index + 1);

						int space_index = TempString.IndexOf(" ", 0);

						if( space_index >= 0 )
						{
							ArgString = TempString.Substring(space_index + 1);

							ArgString = ArgString.Replace("$F", FilePath);
							ArgString = ArgString.Replace("$L", string.Format("{0}", LineNumber));
						}
					}
				}
				else
				{
					int space_index = TempString.IndexOf(" ", 0);

					if( space_index > 0 )
					{
						ProcString = TempString.Substring(0, space_index);

						ArgString = TempString.Substring(space_index + 1);

						ArgString = ArgString.Replace("$F", FilePath);
						ArgString = ArgString.Replace("$L", string.Format("{0}", LineNumber));
					}
					else
					{
						ProcString = TempString;
					}
				}

				if( ArgString == "" )
				{
					ArgString = FilePath;
				}

				if( ProcString != "" )
				{
					if( File.Exists(ProcString) )
					{
						try
						{
							ProcessStartInfo pi = new ProcessStartInfo(ProcString);
							pi.WorkingDirectory = Path.GetDirectoryName(FilePath);
							pi.Arguments = ArgString;
							pi.UseShellExecute = false;
							Process.Start(pi);
						}
						catch(Exception ex)
						{
							MessageBox.Show(string.Format("Error occurred running '{0}'\n({1})", ProcString, ex.Message), "Error running executable");
						}
					}
					else
					{
						MessageBox.Show(string.Format("Custrom editor '{0}' does not exist.", ProcString), "File Not Found");
					}
				}
			}
		}

		private void CreateWorkerThreads()
		{
			// kill the worker threads...
			if( Globals.Workers != null )
			{
				for( int index = 0; index < Globals.Workers.Count(); index++ )
				{
					if( WorkerThreads[index] != null )
					{
						WorkerThreads[index].Abort();
					}
				}
			}

			// create array of worker structs and an array for the threads
			Globals.Workers = new Globals.Worker_s[Globals.NumWorkerThreads];
			WorkerThreads = new Thread[Globals.Workers.Count()];

			for( int index = 0; index < Globals.Workers.Count(); index++ )
			{
				// create a new Worker class instance and create the Worker thread
				WorkerThreads[index] = new Thread(new Worker(Handle, index).Run);
				WorkerThreads[index].Priority = ThreadPriority.BelowNormal;
				WorkerThreads[index].Start();  // start the thread running
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Globals.MainFormHandle = Handle;
			RichTextBox.FormHandle = new HandleRef(this, Globals.MainFormHandle);

			// set FileListView to double buffered to reduce flicker
			// (don't do this for Remote Desktop connections because it will slow them down.  https://blogs.msdn.microsoft.com/oldnewthing/20060103-12/?p=32793)
			if( !System.Windows.Forms.SystemInformation.TerminalServerSession ) 
			{
			    System.Reflection.PropertyInfo Prop = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			    Prop.SetValue(FileListView, true, null);
			}

			ResendMessageList = new List<int>();

			stopSearchToolStripMenuItem.Visible = false;
			stopSearchToolStripMenuItem.Enabled = false;

			searchStatusLabel.Text = "";

			SearchCountLabel.Text = "";
			SearchCountLabel.Enabled = false;
			SearchCountLabel.Visible = false;

			CollectFilesTimer = new System.Timers.Timer();
			CollectFilesTimer.Elapsed += new ElapsedEventHandler(OnCollectFilesTimerEvent);
			CollectFilesTimer.AutoReset = false;

			SliderUpdateTimer = new System.Timers.Timer();
			SliderUpdateTimer.Elapsed += new ElapsedEventHandler(OnSliderUpdateTimerTimerEvent);
			SliderUpdateTimer.AutoReset = false;

			columnSorter = new ListViewColumnSorter();

			ColumnWidthSize = new int[6];

			// create a right-click menu in the FileListView to perform various functions ("copy files to clipboard", etc.)
			MenuItem[] flv_mi = new MenuItem[] { new MenuItem("Copy Filenames To Clipboard"), new MenuItem("Export to CSV File") };

			for( int i = 0; i < flv_mi.Length; i++ )
			{
				flv_mi[i].Click += OnFileListView_MenuItemClick;
			}

			FileListView.ContextMenu = new ContextMenu(flv_mi);

			// create a right-click menu in the RichTextBox to perform various functions ("copy to clipboard", etc.)
			MenuItem[] rtb_mi = new MenuItem[] { new MenuItem("Select Current Line"), new MenuItem("Select All"), new MenuItem("Copy Selection To Clipboard"), new MenuItem("Open in Editor") };
			for( int i = 0; i < rtb_mi.Length; i++ )
			{
				rtb_mi[i].Click += OnRichTextBox_MenuItemClick;
			}

			RichTextBox.ContextMenu = new ContextMenu(rtb_mi);


			int NumberOfProcessors = Math.Max(Environment.ProcessorCount - 1, 1);  // subtract one from total numberr of logical processors (so we don't max out the CPU)

			Globals.NumWorkerThreads = Math.Min(3, NumberOfProcessors);  // default to 3 worker threads (unless CPU has fewer cores)
			Config.Get(Config.KEY.NumWorkerThreads, ref Globals.NumWorkerThreads);

			if( Globals.NumWorkerThreads > NumberOfProcessors )  // clamp the number of worker threads at this system's max (config could be higher if .ini file copied from other machine)
			{
				Globals.NumWorkerThreads = NumberOfProcessors;
			}

			CreateWorkerThreads();

			// create a new GetFiles class instance and create the GetFiles thread
			GetFilesThread = new Thread(new GetFiles(Handle).Run);
			GetFilesThread.Priority = ThreadPriority.BelowNormal;
			GetFilesThread.Start();  // start the thread running

			int splitter_type = 0;  // 0 = horizontal
			if( Config.Get(Config.KEY.SplitterType, ref splitter_type) )
			{
				splitContainer1.Orientation = (splitter_type == 0) ? Orientation.Horizontal : Orientation.Vertical;
			}

			if( splitContainer1.Orientation == Orientation.Horizontal )
			{
				horizontalSplitToolStripMenuItem.Checked = true;
				verticalSplitToolStripMenuItem.Checked = false;
				int splitter_distance = 0;
				if( Config.Get(Config.KEY.SplitterHorizontalDistance, ref splitter_distance) )
				{
					splitContainer1.SplitterDistance = splitter_distance;
				}
			}
			else
			{
				horizontalSplitToolStripMenuItem.Checked = false;
				verticalSplitToolStripMenuItem.Checked = true;
				int splitter_distance = 0;
				if( Config.Get(Config.KEY.SplitterVerticalDistance, ref splitter_distance) )
				{
					splitContainer1.SplitterDistance = splitter_distance;
				}
			}

			bool bDisplayLineNumbers = true;
			if( Config.Get(Config.KEY.DisplayLineNumbers, ref bDisplayLineNumbers) )
			{
				displayLineNumbersToolStripMenuItem.Checked = bDisplayLineNumbers;
			}

			SetRichTextTabPositions();

			List<int> ListViewColumnWidth = new List<int>();
			if( Config.Get(Config.KEY.ListViewColumnWidth, ref ListViewColumnWidth) )
			{
				for( int index = 0; index < ListViewColumnWidth.Count; index++ )
				{
					FileListView.Columns[index].Width = ListViewColumnWidth[index];
				}
			}

			for( int column_index = 0; column_index < 6; column_index++ )
			{
				ColumnWidthSize[column_index] = FileListView.Columns[column_index].Width;
			}

			int preview_before_slider = -1;
			if( Config.Get(Config.KEY.PreviewBeforeSlider, ref preview_before_slider) )
			{
				PreviewBeforeTrackBar.Value = preview_before_slider;
			}

			int preview_after_slider = -1;
			if( Config.Get(Config.KEY.PreviewAfterSlider, ref preview_after_slider) )
			{
				PreviewAfterTrackBar.Value = preview_after_slider;
			}

			bDeferRichTextDisplay = false;
			Config.Get(Config.KEY.OptionsDeferRichTextDisplay, ref bDeferRichTextDisplay);

			bUseWindowsFileAssociation = true;
			if( Config.Get(Config.KEY.OptionsUseWindowsFileAssociation, ref bUseWindowsFileAssociation) )
			{
				if( !bUseWindowsFileAssociation )
				{
					CustomEditorString = "";
					Config.Get(Config.KEY.OptionsCustomEditor, ref CustomEditorString);

					if( CustomEditorString == "" )
					{
						bUseWindowsFileAssociation = true;
					}
				}
			}

			Font ListViewFont = null;
			if (Config.Get(Config.KEY.FileListFont, ref ListViewFont))
			{
				FileListView.Font = ListViewFont;
			}

			Font SearchResultsFont = null;
			if (Config.Get(Config.KEY.SearchResultsFont, ref SearchResultsFont))
			{
				RichTextBox.Font = SearchResultsFont;
				RichTextBox.SelectionFont = SearchResultsFont;
			}

			if ( fileToolStripMenuItem.Selected )  // if the 'File' menu item is selected by default (because it's the first in the Tab Order)
			{
				FileListView.Focus();  // set focus to the FileListView instead
			}

			bWindowInitComplete = true;  // window initialization is complete, okay to write config settings now
		}

		private void MainForm_Closing(object sender, FormClosingEventArgs e)
		{
			// save window maximized state and write config settings on exit...
			Config.Set(Config.KEY.Maximized, (this.WindowState == FormWindowState.Maximized));

			Config.Save();

			// kill the worker threads and the GetFiles thread...
			if( Globals.Workers != null )
			{
				for( int index = 0; index < Globals.Workers.Count(); index++ )
				{
					if( WorkerThreads[index] != null )
					{
						WorkerThreads[index].Abort();
					}
				}
			}

			if( GetFilesThread != null )
			{
				GetFilesThread.Abort();
			}
		}

		private void SetRichTextTabPositions()
		{
			// get width of character in RichTextBox
			Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
			Size font_sz = new Size(1,1);  // this doesn't seem to matter
			Size sz = TextRenderer.MeasureText(graphics, "W", RichTextBox.Font, font_sz, TextFormatFlags.NoPadding);
			graphics.Dispose();

			TabPositions = new int[32];  // RichTextBox only supports 32 tab positions

			// set the RichText tab positions (assumes tab size of 4 characters)
			int tab_size = 4 * sz.Width;

			for(int x = 0; x < 32; x++)
			{
				if (displayLineNumbersToolStripMenuItem.Checked)
				{
					TabPositions[x] = (sz.Width * 10) + ((x+1) * tab_size);  // we prepend "{0,8}: " (10 characters) for the line number at the beginning of lines, offset by this many characters for first tab stop
				}
				else
				{
					TabPositions[x] = (x+1) * tab_size;
				}
			}
		}

		private void StartCollectingFilesForSearch()
		{
			GC.Collect();  // collect garbage before each search

			searchCtrlFToolStripMenuItem.Enabled = false;
			optionsToolStripMenuItem.Enabled = false;

			allSearchMatchesToolStripMenuItem.Enabled = false;

			bIsSearchInProgress = false;  // this gets set AFTER we have collected the files to be searched
			bDisableListViewSorting = true;  // disable the ability to sort the ListView table until we are done processing files

			TotalNumberOfSearchMatches = 0;
			TotalNumberOfSearchedFiles = 0;
			TotalNumberOfSearchFileLines = 0;

			SearchCountLabel.Text = "";
			SearchCountLabel.Enabled = false;
			SearchCountLabel.Visible = false;

			searchStatusLabel.Text = "";

			FileListView.Items.Clear();
			FileListView.ListViewItemSorter = null;  // don't sort the items as we add them (we want them unsorted by default)
			FileListViewSelectedItem = null;

			ListViewFileClickedOnIndex = -1;  // we haven't clicked on a file from the ListView yet

			RichTextBox.Clear();

			if ((TabPositions != null) && (TabPositions.Length > 0))
			{
				RichTextBox.SelectionTabs = TabPositions;
			}

			Globals.GetFiles.bShouldStopCurrentJob = false;
			Globals.GetFiles.WaitHandle.Set();  // tell the GetFiles thread to collect the files to search

			collectingFilesDialog = new CollectingFiles();

			collectingFilesDialog.StartPosition = FormStartPosition.CenterParent;
			collectingFilesDialog.ControlBox = false;  // don't show the 'X' to close button

			collectingFilesDialog.ShowDialog();
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			if( (Globals.SearchDirectory != "") && (Globals.SearchString != "") && (Globals.FileSpecs != null) && (Globals.FileSpecs.Count > 0) )
			{
				StartCollectingFilesForSearch();
				return;
			}

			if( Globals.SearchDirectory != "" )
			{
				DoSearchDialog();
			}
		}

		private void MainForm_Move(object sender, EventArgs e)
		{
			if( bShouldSaveMoveLocation && bWindowInitComplete )
			{
				Config.Set(Config.KEY.PosX, Location.X);
				Config.Set(Config.KEY.PosY, Location.Y);
			}

			bShouldSaveMoveLocation = true;
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if( bShouldSaveResizeSize && bWindowInitComplete )
			{
				Control control = (Control)sender;
				Config.Set(Config.KEY.Width, control.Size.Width);
				Config.Set(Config.KEY.Height, control.Size.Height);
			}

			bShouldSaveResizeSize = true;
		}

		private void DoSearchDialog()
		{
			SearchForm searchDialog = new SearchForm();

			searchDialog.ShowDialog(this);

			if( searchDialog.DialogResult == DialogResult.OK )
			{
				Globals.SearchDirectory = searchDialog.FolderName;
				Globals.SearchString = searchDialog.SearchFor;
				Globals.FileSpecs = searchDialog.FileSpec.Split(' ').ToList();

				Globals.bRegEx = searchDialog.bRegularExpression;
				Globals.bCaseSensitive = searchDialog.bMatchCase;
				Globals.bRecursive = searchDialog.bIsRecursive;

				StartCollectingFilesForSearch();
			}
		}

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if( bWindowInitComplete )
			{
				if( splitContainer1.Orientation == Orientation.Horizontal )
				{
					Config.Set(Config.KEY.SplitterHorizontalDistance, splitContainer1.SplitterDistance);
				}
				else
				{
					Config.Set(Config.KEY.SplitterVerticalDistance, splitContainer1.SplitterDistance);
				}
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void searchCtrlFToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Globals.SearchDirectory = "";

			DoSearchDialog();
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OptionsForm optionsDialog = new OptionsForm(FileListView.Font, RichTextBox.Font);

			optionsDialog.ShowDialog(this);

			if( optionsDialog.DialogResult == DialogResult.OK )
			{
				bDeferRichTextDisplay = optionsDialog.DeferRichText;
				bUseWindowsFileAssociation = optionsDialog.UseWindowsFileAssociation;

				if( !bUseWindowsFileAssociation )
				{
					CustomEditorString = optionsDialog.CustomEditorText;

					if( CustomEditorString == "" )
					{
						bUseWindowsFileAssociation = true;
					}
				}

				if( optionsDialog.NumberOfWorkerThreads != Globals.NumWorkerThreads )  // did the user change the number of worker threads?
				{
					Globals.NumWorkerThreads = optionsDialog.NumberOfWorkerThreads;

					CreateWorkerThreads();

					Config.Set(Config.KEY.NumWorkerThreads, Globals.NumWorkerThreads);
				}

				if( optionsDialog.bListViewFontChanged)
				{
					FileListView.Font = optionsDialog.ListViewFont;
					FileListView.Invalidate();
				}

				if( optionsDialog.bRichTextBoxFontChanged)
				{
					RichTextBox.Font = optionsDialog.RichTextBoxFont;
					RichTextBox.SelectionFont = optionsDialog.RichTextBoxFont;

					ReDisplayRichTextSearchMatches(ListViewFileClickedOnIndex);
				}
			}
		}

		private void horizontalSplitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bWindowInitComplete = false;  // we are in the process of redrawing the window, don't save config settings

			horizontalSplitToolStripMenuItem.Checked = true;
			verticalSplitToolStripMenuItem.Checked = false;

			splitContainer1.Orientation = Orientation.Horizontal;

			Config.Set(Config.KEY.SplitterType, 0);  // 0 = horizontal

			int splitter_distance = 0;
			if( Config.Get(Config.KEY.SplitterHorizontalDistance, ref splitter_distance) )
			{
				splitContainer1.SplitterDistance = splitter_distance;
			}
			else
			{
				splitContainer1.SplitterDistance = splitContainer1.Size.Height / 2;
				Config.Set(Config.KEY.SplitterHorizontalDistance, splitContainer1.SplitterDistance);
			}

			bWindowInitComplete = true;
		}

		private void verticalSplitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bWindowInitComplete = false;  // we are in the process of redrawing the window, don't save config settings

			horizontalSplitToolStripMenuItem.Checked = false;
			verticalSplitToolStripMenuItem.Checked = true;

			splitContainer1.Orientation = Orientation.Vertical;

			Config.Set(Config.KEY.SplitterType, 1);  // 1 = vertical

			int splitter_distance = 0;
			if( Config.Get(Config.KEY.SplitterVerticalDistance, ref splitter_distance) )
			{
				splitContainer1.SplitterDistance = splitter_distance;
			}
			else
			{
				splitContainer1.SplitterDistance = splitContainer1.Size.Width / 2;
				Config.Set(Config.KEY.SplitterVerticalDistance, splitContainer1.SplitterDistance);
			}

			bWindowInitComplete = true;
		}

		private void allSearchMatchesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// if we cancelled the RichText display on the previous pass or we aren't currently displaying all matches...
			if( bWasPreviousRichTextDisplayCancelled || (ListViewFileClickedOnIndex != -1) )
			{
				if( TotalNumberOfSearchFileLines > 1000 )
				{
					PleaseWaitDeferredIndex = -1;
					PleaseWaitForceDisplay = false;

					pleaseWaitDialog = new PleaseWait();

					pleaseWaitDialog.StartPosition = FormStartPosition.CenterParent;
					pleaseWaitDialog.ControlBox = false;  // don't show the 'X' to close button

					pleaseWaitDialog.ShowDialog();
				}
				else
				{
					ReDisplayRichTextSearchMatches(-1);  // re-display all files
				}
			}
		}

		private void displayLineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			displayLineNumbersToolStripMenuItem.Checked = !displayLineNumbersToolStripMenuItem.Checked;

			Config.Set(Config.KEY.DisplayLineNumbers, displayLineNumbersToolStripMenuItem.Checked);

			SetRichTextTabPositions();

			ReDisplayRichTextSearchMatches(ListViewFileClickedOnIndex);
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, "Grepy2Help.chm");
		}

		private void stopSearchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// cancel any search in progress
			if( bIsSearchInProgress )
			{
				stopSearchToolStripMenuItem.Visible = false;
				stopSearchToolStripMenuItem.Enabled = false;

				Globals.bShouldStopWorkerJobs = true;
			}
		}

		private void OnCollectFilesTimerEvent(object source, ElapsedEventArgs e)
		{
			Invoke((MethodInvoker)delegate {SearchingProgressBar.Value = 0;});
			Invoke((MethodInvoker)delegate {SearchingLabel.Enabled = false;});
			Invoke((MethodInvoker)delegate {SearchCountLabel.Enabled = true;});

			CollectFilesTimer.Enabled = false;
		}

		private void OnSliderUpdateTimerTimerEvent(object source, ElapsedEventArgs e)
		{
			SliderUpdateTimer.Enabled = false;

			HandleRef FormHandle = new HandleRef(this, Globals.MainFormHandle);
			PostMessage(FormHandle, Globals.WM_SLIDERUPDATE_NOTIFY_MAIN_THREAD, IntPtr.Zero, IntPtr.Zero);
		}

		private void PreviewBeforeTrackBar_ValueChanged(object sender, EventArgs e)
		{
			if( bWindowInitComplete )
			{
				Config.Set(Config.KEY.PreviewBeforeSlider, PreviewBeforeTrackBar.Value);

				if( TotalNumberOfSearchMatches > 0 )
				{
					SliderUpdateTimer.Interval = 1000;  // expire in 1 second
					SliderUpdateTimer.Enabled = true;
				}
			}
		}

		private void PreviewAfterTrackBar_ValueChanged(object sender, EventArgs e)
		{
			if( bWindowInitComplete )
			{
				Config.Set(Config.KEY.PreviewAfterSlider, PreviewAfterTrackBar.Value);

				if( TotalNumberOfSearchMatches > 0 )
				{
					SliderUpdateTimer.Interval = 1000;  // expire in 1 second
					SliderUpdateTimer.Enabled = true;
				}
			}
		}

		private void FileListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if( !bDisableListViewSorting )
			{
				columnSorter.SetCaseSensitiveColumnSort(false);  // set the column case sensitive sort setting

				// Reverse the current sort direction for this column.
				if( e.Column == columnSorter.SortColumn )
				{
					// Reverse the current sort direction for this column.
					if( columnSorter.Order == SortOrder.Ascending)
					{
						columnSorter.Order = SortOrder.Descending;
					}
					else
					{
						columnSorter.Order = SortOrder.Ascending;
					}
				}
				else
				{
					// Set the column number that is to be sorted; default to ascending.
					columnSorter.SortColumn = e.Column;
					columnSorter.Order = SortOrder.Ascending;
				}

				if( columnSorter.Order != SortOrder.None )
				{
					columnSorter.SetSortIcon(FileListView);
				}

				// Perform the sort with these new sort options.
				FileListView.Sort();
			}
		}

		private void FileListView_DoubleClick(object sender, EventArgs e)
		{
			ListView.SelectedListViewItemCollection Selected = FileListView.SelectedItems;

			foreach( ListViewItem item in Selected )
			{
				if( item.SubItems[0].Text != "" )  // is filename present?
				{
					string FilePath = string.Format("{0}\\{1}", item.SubItems[2].Text, item.SubItems[0].Text);

					OpenFile(FilePath, 1);
				}
			}
		}

		private void FileListView_MouseClick(object sender, MouseEventArgs e)
		{
			ListViewItem ClickedItem = null;

			foreach (ListViewItem item in FileListView.Items)
			{
				if( item.Bounds.Contains(e.Location) )
				{
					ClickedItem = item;
					break;
				}
			}

			if( ClickedItem == null )
			{
				return;
			}

			if( e.Button == MouseButtons.Right )
			{
				if( FileListViewSelectedItem != null )
				{
					FileListViewSelectedItem.Selected = true;
					FileListViewSelectedItem.Focused = true;
				}
			}
			else if( e.Button == MouseButtons.Left )
			{
				if( FileListViewSelectedItem != null)
				{
					FileListViewSelectedItem.BackColor = FileListViewBackColor;
				}

				FileListViewSelectedItem = ClickedItem;
				FileListViewBackColor = ClickedItem.BackColor;  // save the background color so we can restore it later
				ClickedItem.BackColor = Color.WhiteSmoke;

				ClickedItem.Selected = true;

				ReDisplayRichTextSearchMatches(Convert.ToInt32(ClickedItem.SubItems[6].Text));  // re-display the file for this index
			}
		}

		private void FileListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if( e.IsSelected && e.Item != FileListViewSelectedItem )
			{
				e.Item.Selected = false;
			}
		}

		private void FileListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			if( bWindowInitComplete )
			{
				bool bHasColumnWidthChanged = false;

				for( int column_index = 0; column_index < 6; column_index++ )
				{
					if( ColumnWidthSize[column_index] != FileListView.Columns[column_index].Width )
					{
						bHasColumnWidthChanged = true;
					}
				}

				if( bHasColumnWidthChanged )
				{
					List<int> ListViewColumnWidth = new List<int>();

					for( int index = 0; index < 6; index++ )  // there are 6 visible columns in the ListView
					{
						int width = FileListView.Columns[index].Width;
						ColumnWidthSize[index] = width;  // save the new column width for later comparison
						ListViewColumnWidth.Add(width);
					}

					Config.Set(Config.KEY.ListViewColumnWidth, ListViewColumnWidth);
				}
			}
		}

		private void OnFileListView_MenuItemClick(object sender, EventArgs e)
		{
			int index = ((MenuItem)sender).Index;

			if( index == 0 )  // copy filenames to clipboard
			{
				string filename_text = "";

				foreach (ListViewItem item in FileListView.Items)
				{
					if( filename_text != "" )
					{
						filename_text += "\n";
					}

					filename_text += string.Format("{0}\\{1}", item.SubItems[2].Text, item.SubItems[0].Text);
				}

				if( filename_text != "" )
				{
					Clipboard.SetText(filename_text, TextDataFormat.Text);
				}
			}
			else if( index == 1 )  // export to comma separated values (CSV) file
			{
				SaveFileDialog dialog = new SaveFileDialog();
				dialog.Filter = "CSV files (*.csv)|*.csv";

				if( dialog.ShowDialog() == DialogResult.OK )
				{
					try
					{
						using (StreamWriter sw = new StreamWriter(dialog.FileName))
						{
							foreach (ListViewItem item in FileListView.Items)
							{
								string output = string.Format("\"{0}\\{1}\"", item.SubItems[2].Text, item.SubItems[0].Text);
								output += string.Format(",{0}", item.SubItems[1].Text);  // filetype
								output += string.Format(",{0}", item.SubItems[3].Text);  // search text matches
								output += string.Format(",{0}", item.SubItems[4].Text);  // filesize
								output += string.Format(",{0}", item.SubItems[5].Text);  // date/time

								sw.WriteLine(output);
							}
						}
					}
					catch( Exception )
					{
						Console.WriteLine("Error saving CSV file!");
					}
				}
			}
		}

		private void OnRichTextBox_MenuItemClick(object sender, EventArgs e)
		{
			int index = ((MenuItem)sender).Index;

			if( (index == 0) || (index == 1) )
			{
				// save the scroll bar thumb positions
				int horiz_pos = GetScrollPos(RichTextBox.Handle, System.Windows.Forms.Orientation.Horizontal);
				int vert_pos = GetScrollPos(RichTextBox.Handle, System.Windows.Forms.Orientation.Vertical);

				if( index == 0 )  // select the line at the cursor
				{
					int CharacterIndex = RichTextBox.GetCharIndexFromPosition(RichTextBox.RichTextMousePosition);
					int LineIndex = RichTextBox.GetLineFromCharIndex(CharacterIndex);
					if( LineIndex < RichTextBox.Lines.Length )
					{
						int start = RichTextBox.GetFirstCharIndexFromLine(LineIndex);
						RichTextBox.Select(start, RichTextBox.Lines[LineIndex].Length);
					}
				}
				else if( index == 1 )  // select all
				{
					RichTextBox.SelectAll();  // this will select all text and move the caret (and scroll to the end)
				}

				// notify the RichTextBox that the scroll bar thumb positions have changed (to put the text back where it was before the select)
				int wParamHoriz = (horiz_pos << 16) + SB_THUMBPOSITION;
				IntPtr wParamHorizPtr = new IntPtr(wParamHoriz);
				SendMessage(RichTextBox.Handle, WM_HSCROLL, wParamHorizPtr, IntPtr.Zero);

				int wParamVert = (vert_pos << 16) + SB_THUMBPOSITION;
				IntPtr wParamVertPtr = new IntPtr(wParamVert);
				SendMessage(RichTextBox.Handle, WM_VSCROLL, wParamVertPtr, IntPtr.Zero);
			}
			else if( index == 2 )  // copy selection to clipboard
			{
				if( RichTextBox.SelectionLength > 0 )
				{
					Clipboard.SetText(RichTextBox.SelectedText, TextDataFormat.Text);
				}
			}
			else if( index == 3 )  // open in editor
			{
				OpenFileFromRichTextBoxPoint(RichTextBox.RichTextMousePosition);
			}
		}
	}
}
