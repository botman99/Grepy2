using System;
using System.Collections.Generic;

using System.Threading;

namespace Grepy2
{
	// NOTE: We don't need any locking for the globals shared between the Main Form thread and any of the other threads.
	// The other threads will wait on their 'WaitHandle' and the Main Form thread won't Set() the handle until it has fully
	// initialized any variables needed by the other thread.  Once Set() has been called, the main thread won't modify
	// any of these other thread variables.  The main thread will wait until the other thread sends a Windows Message
	// to the WndProc() function to indicate that the other thread is done doing work.  The other thread will fully
	// initialize any global variables that will be read by the main thread before the other thread sends the Windows Message.

	static class Globals
	{
		public const int WM_USER = 0x0400;  // Windows private user messages
		public const int WM_GETFILES_NOTIFY_MAIN_THREAD = WM_USER;  // notify main thread that collecting files is done
		public const int WM_WORKER_NOTIFY_MAIN_THREAD = WM_USER+1;  // notify main thread that worker has searched file for search text
		public const int WM_SLIDERUPDATE_NOTIFY_MAIN_THREAD = WM_USER+2;  // notify main thread that the Before/After preview slider has updated
		public const int WM_PLEASEWAITSHOWN_NOTIFY_MAIN_THREAD = WM_USER+3;  // notify main thread that the "Please Wait" dialog has been shown
		public const int WM_CTRL_CLICK_NOTIFY_MAIN_THREAD = WM_USER+4;  // notify main thread that RichTextBox Ctrl Single Click happened

		public struct GetFiles_s  // struct for GetFiles thread
		{
			public AutoResetEvent WaitHandle;
			public bool bIsWorking;  // is thread currently working on a job (set by thread, read by others)
			public bool bShouldStopCurrentJob;  // thread should stop working on current job (cancel work, set by Main Form, ready by GetFiles thread)
			public bool bShouldExit;  // thread should exit (quit)
		}

		public static GetFiles_s GetFiles;


		public struct Worker_s  // struct for Worker threads
		{
			public AutoResetEvent WaitHandle;
			public bool bIsWorking;  // is thread currently working on a job (set by thread, read by others)
			public bool bShouldExit;  // thread should exit (quit)

			public int SearchFilesIndex;  // index into the SearchFiles list that is assigned to this worker
		}

		public static Worker_s[] Workers;


		public enum SearchFileStatus
		{
			NotProcessed = 0,
			FileIsBinary,
			SearchTextNotFound,
			SearchTextFound
		};

		public class SearchMatchPosition  // class for holding start position and length of a search text match within a line
		{
			public int StartPos;
			public int Length;
		}

		public class SearchLine  // class for holding a line of text from the file
		{
			public int LineNumber;  // line number in the file
			public string Line;  // line of text from the file
			public bool bIsSearchTextMatch;  // does this line contain a match for the search text (if not, it's a preview line above/below a match)
			public List<SearchMatchPosition> SearchMatchPositions;  // holds the start position and length of search text matches (there could be multiple matches per line of text)
			public int LinesBeforeNextMatch;  // the number of lines between this line and the next search text match line in the SearchFile_s 'Lines' list (for the RichText display of lines without matches)
		}

		public class SearchFile  // class for a file that will be searched for the search text
		{
			public SearchFileStatus Status;  // has this file been processed yet by the worker threads (some files searched will contain no matches)
			public string Filename;  // this is the full path filename
			public string BaseFilename;
			public string FileTypeName;
			public string FolderName;
			public int SearchMatchCount;  // number of times the search text was found in this file
			public long FileLength;
			public DateTime FileLastWriteTime;
			public List<SearchLine> Lines;
			public int Matches;
		}

		public static SearchFile[] SearchFiles;
		public static int SearchDirectoryCount;  // number of directories we searched through to collect files to search

		public static bool bIsGrepyReadOnly = false;
		public static bool bIsEverythingRunning = false;
		public static uint EverythingMajor = 0;
		public static uint EverythingMinor = 0;
		public static uint EverythingRevision = 0;

		public static string ApplicationPathExe;
		public static int NumWorkerThreads;

		public static bool bShouldStopWorkerJobs = false;  // cancel all worker jobs in progress

		public static bool bPleaseWaitDialogCancelled;

		// initialized by main (user) Form thread...
		public static IntPtr MainFormHandle;
		public static bool bRecursive = false;  // should the GetFiles search be recursive?
		public static string SearchDirectory = "";
		public static string SearchString = "";
		public static List<string> FileSpecs;
		public static bool bRegEx = false;  // should the text search use regular expressions?
		public static bool bCaseSensitive = false;  // should the text search be case sensitive?

		static Globals()
		{
			// do nothing
		}
	}
}
