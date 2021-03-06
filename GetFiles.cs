﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace Grepy2
{
	class GetFiles
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("Everything32.dll")]
		public static extern void Everything_Reset();
		[DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
		public static extern int Everything_SetSearchW(string lpSearchString);
		[DllImport("Everything32.dll")]
		public static extern void Everything_SortResultsByPath();
		[DllImport("Everything32.dll")]
		public static extern bool Everything_QueryW(bool bWait);
		[DllImport("Everything32.dll")]
		public static extern int Everything_GetNumResults();
		[DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
		public static extern void Everything_GetResultFullPathNameW(int nIndex, StringBuilder lpString, int nMaxCount);
		[DllImport("Everything32.dll")]
		public static extern bool Everything_IsDBLoaded();

		private static HandleRef FormHandle;

		List<string> filenames;
		List<string> folders;

		public GetFiles(IntPtr InHandle)
		{
			FormHandle = new HandleRef(this, InHandle);

			Globals.GetFiles.WaitHandle = new AutoResetEvent(false);  // create a WaitHandle for syncronization

			Globals.GetFiles.bIsWorking = false;
			Globals.GetFiles.bShouldStopCurrentJob = false;
			Globals.GetFiles.bShouldExit = false;
		}

		public void Run()
		{
			while( !Globals.GetFiles.bShouldExit )
			{
				Globals.GetFiles.WaitHandle.WaitOne();  // wait for something to do

				Globals.GetFiles.bIsWorking = true;

				Globals.SearchDirectoryCount = 0;

				filenames = new List<string>();
				folders = new List<string>();

				bool bUseInternalGetFiles = true;  // assume we want to use the GetFiles method if Everything fails

				// if using the Everything search engine and not searching a Microsoft UNC (Universal Naming Convention) file path...
				if( Globals.bIsEverythingRunning && !Globals.SearchDirectory.StartsWith("\\\\") )
				{
					if (Everything_IsDBLoaded())
					{
						Everything_Reset();

						string everything_search;

						string search_drive = Globals.SearchDirectory.Substring(0, 2);

						// see if the drive is indexed by Everything by checking for the existance of any folder or file in the root (if zero results are returned, the drive isn't indexed, or is empty)
						everything_search = string.Format("\"{0}\\*\" parents:1", search_drive);

						Everything_SetSearchW(everything_search);  // set the search parameter (quoted folder name followed by '<' 

						Thread.Sleep(0);  // force context switch

						bool bDriveIsIndexed = false;

						if( Everything_QueryW(true) )  // wait for the results
						{
							if( Everything_GetNumResults() > 0)
							{
								bDriveIsIndexed = true;
							}
						}

						if (bDriveIsIndexed)
						{
							Everything_Reset();

							// trim off any trailing backslashes on the search directory
							string search_directory = Globals.SearchDirectory.TrimEnd('\\');

							if( Globals.bRecursive )
							{
								everything_search = string.Format("\"{0}\\*\" file:wholefilename:nocase:noregex:<", search_directory);
							}
							else
							{
								string[] paths = search_directory.Split('\\');
								everything_search = string.Format("\"{0}*\" parents:{1} file:wholefilename:nocase:noregex:<", search_directory, paths.Length);
							}

							for( int i = 0; i < Globals.FileSpecs.Count; i++ )
							{
								if( i > 0 )
								{
									everything_search += " | ";
								}
								everything_search += Globals.FileSpecs[i];
							}

							everything_search = everything_search + ">";

							Everything_SetSearchW(everything_search);  // set the search parameter (quoted folder name followed by '<' 

							Thread.Sleep(0);  // force context switch

							if( Everything_QueryW(true) )  // wait for the results
							{
								bUseInternalGetFiles = false;  // don't use the GetFiles thread

								Everything_SortResultsByPath();  // sort the results by path

								int SearchFilesCount = Everything_GetNumResults();

								if( SearchFilesCount > 0 )
								{
									const int bufsize = 260; 
									StringBuilder buf = new StringBuilder(bufsize);

									for( int i = 0; i < SearchFilesCount; i++ )
									{
										Everything_GetResultFullPathNameW(i, buf, bufsize);

										filenames.Add(buf.ToString());
									}
								}
							}
						}
					}
				}

				if( bUseInternalGetFiles )
				{
					InternalGetFiles(Globals.SearchDirectory);
				}

				if( !Globals.GetFiles.bShouldExit )
				{
					Globals.SearchFiles = new Globals.SearchFile[filenames.Count];

					for( int i = 0; (i < filenames.Count) && !Globals.GetFiles.bShouldStopCurrentJob; i++ )
					{
						Globals.SearchFile search_file = new Globals.SearchFile();

						search_file.Status = Globals.SearchFileStatus.NotProcessed;
						search_file.Filename = filenames[i];
						search_file.BaseFilename = Path.GetFileName(filenames[i]);
						search_file.FileTypeName = "";
						search_file.FolderName = Path.GetDirectoryName(filenames[i]);
						search_file.SearchMatchCount = 0;
						search_file.FileLength = 0;
						search_file.FileLastWriteTime = DateTime.MinValue;
						search_file.Lines = null;
						search_file.Matches = 0;

						Globals.SearchFiles[i] = search_file;

						if( !folders.Contains(search_file.FolderName) )  // keep track of the number of folders that will need to be searched
						{
							folders.Add(search_file.FolderName);
						}
					}

					Globals.SearchDirectoryCount = folders.Count;

					Globals.GetFiles.bIsWorking = false;

					PostMessage(FormHandle, Globals.WM_GETFILES_NOTIFY_MAIN_THREAD, IntPtr.Zero, IntPtr.Zero);  // notify main Form thread that we are done (or were cancelled)
				}
			}
		}

		private List<string> GetFilesForDirectory(string path, string filespec)
		{
			List<string> list = new List<string>();

			try
			{
				list.AddRange(Directory.GetFiles(path, filespec, SearchOption.TopDirectoryOnly));

				if( Globals.GetFiles.bShouldStopCurrentJob || Globals.GetFiles.bShouldExit )
				{
					return null;
				}

				if( Globals.bRecursive )
				{
					string[] dirlist = Directory.GetDirectories(path);

					if( dirlist != null )
					{
						for( int i=0; i < dirlist.Length; i++ )
						{
							if( Globals.GetFiles.bShouldStopCurrentJob || Globals.GetFiles.bShouldExit )
							{
								return null;
							}

							list.AddRange(GetFilesForDirectory(dirlist[i], filespec));
						}
					}
				}
			}
			catch( Exception e)
			{
				Console.WriteLine("GetFilesForDirectory() - Exception: {0}", e.Message);
			}

			if( Globals.GetFiles.bShouldStopCurrentJob || Globals.GetFiles.bShouldExit )
			{
				return null;
			}

			return list;
		}

		private void InternalGetFiles(string InDirectory)
		{
			for( int index = 0; index < Globals.FileSpecs.Count; index++ )
			{
				filenames.AddRange(GetFilesForDirectory(InDirectory, Globals.FileSpecs[index]));

				if( Globals.GetFiles.bShouldStopCurrentJob || Globals.GetFiles.bShouldExit )
				{
					return;
				}

				Thread.Sleep(0);  // force context switch
			}
		}
	}
}
