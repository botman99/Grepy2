using System;
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

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

		[DllImport("kernel32.dll")]
		public static extern bool FindClose(IntPtr hFindFile);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WIN32_FIND_DATAW {
			public FileAttributes dwFileAttributes;
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
			internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
			public int nFileSizeHigh;
			public int nFileSizeLow;
			public int dwReserved0;
			public int dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

		[DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
		public static extern bool PathMatchSpecW(string pszFile, string pszSpec);


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

				filenames = GetFilesForDirectory(Globals.SearchDirectory);

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

		private List<string> GetFilesForDirectory(string InDirectory)
		{
			List<string> list = new List<string>();

			IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
			WIN32_FIND_DATAW FindFileData;

			IntPtr hFind = FindFirstFileW(InDirectory + "\\*.*", out FindFileData);
			if (hFind != INVALID_HANDLE_VALUE)
			{
				do
				{
					if( Globals.GetFiles.bShouldStopCurrentJob || Globals.GetFiles.bShouldExit )
					{
						return list;
					}

					if (FindFileData.cFileName == "." || FindFileData.cFileName == "..")
					{
						continue;
					}

					if (Globals.bRecursive && ((FindFileData.dwFileAttributes & FileAttributes.Directory) != 0))
					{
						list.AddRange(GetFilesForDirectory(InDirectory + "\\" + FindFileData.cFileName));

						continue;
					}

					for( int index = 0; index < Globals.FileSpecs.Count; index++ )
					{
						if (PathMatchSpecW(FindFileData.cFileName, Globals.FileSpecs[index]))
						{
							list.Add(InDirectory + "\\" + FindFileData.cFileName);
							break;
						}
					}
				}
				while (FindNextFile(hFind, out FindFileData));

				FindClose(hFind);

				Thread.Sleep(0);  // force context switch
			}

			return list;
		}
	}
}
