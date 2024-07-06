using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace Grepy2
{
	static class Program
	{
		[DllImport("kernel32.dll")]
		public static extern bool AttachConsole(int dwProcessId);
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetStdHandle(int dwStdHandle);
		[DllImport("user32.dll")]
		static extern uint MapVirtualKey(uint uCode, uint uMapType);
		[DllImport("kernel32.dll", EntryPoint = "WriteConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool WriteConsoleInput(IntPtr hConsoleInput, [MarshalAs(UnmanagedType.LPArray), In] INPUT_RECORD[] lpBuffer, int nLength, out int lpNumberOfEventsWritten);

		[StructLayout(LayoutKind.Explicit,CharSet=CharSet.Unicode)]
		public struct KEY_EVENT_RECORD
		{
			[FieldOffset(0),MarshalAs(UnmanagedType.Bool)]
			public bool bKeyDown;
			[FieldOffset(4),MarshalAs(UnmanagedType.U2)]
			public ushort wRepeatCount;
			[FieldOffset(6),MarshalAs(UnmanagedType.U2)]
			public ushort wVirtualKeyCode;
			[FieldOffset(8),MarshalAs(UnmanagedType.U2)]
			public ushort wVirtualScanCode;
			[FieldOffset(10)]
			public char UnicodeChar;
			[FieldOffset(12),MarshalAs(UnmanagedType.U4)]
			public int dwControlKeyState;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT_RECORD
		{
			[FieldOffset(0)]
			public ushort EventType;
			[FieldOffset(4)]
			public KEY_EVENT_RECORD KeyEvent;
		};

		const int STD_INPUT_HANDLE = -10;
		const int KEY_EVENT = 0x0001;

		private static Mutex m_Mutex;
		private static string CommandlineErrorMessage = "";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// if some commandline arguments were given then try to process those arguments...
			if( args.Count() > 0 )
			{
				if( !ProcessCommandlineArguments(args) )  // if we can't process the commandline arguments, then provide help
				{
					AttachConsole(-1);

					IntPtr Handle = GetStdHandle(STD_INPUT_HANDLE);

					Console.WriteLine("");
					Console.WriteLine("");

					if( CommandlineErrorMessage != "" )
					{
						Console.WriteLine(CommandlineErrorMessage);
					}

					Console.WriteLine("");
					Console.WriteLine("Usage: Grepy2.exe [switches] [SearchFolder] SearchString FileSpec [FileSpec...]");
					Console.WriteLine("");
					Console.WriteLine("   where [switches] can be any of the following:");
					Console.WriteLine("   -r = Recurse folders");
					Console.WriteLine("   -cs = Use Case Sensitive searching");
					Console.WriteLine("   -noregex = Don't use regular expressions");
					Console.WriteLine("");

					INPUT_RECORD[] rec = new INPUT_RECORD[2];

					rec[0].EventType = KEY_EVENT;
					rec[0].KeyEvent.bKeyDown = true;
					rec[0].KeyEvent.dwControlKeyState = 0;
					rec[0].KeyEvent.UnicodeChar = '\r';  // carriage return
					rec[0].KeyEvent.wRepeatCount = 1;
					rec[0].KeyEvent.wVirtualKeyCode = 0x0d;  // 0x0d = VK_Return
					uint mvk = MapVirtualKey(0x0d, 0);  // MAPVK_VK_TO_VSC = 0
					rec[0].KeyEvent.wVirtualScanCode = (ushort)mvk;

					rec[1].EventType = KEY_EVENT;
					rec[1].KeyEvent.bKeyDown = false;
					rec[1].KeyEvent.dwControlKeyState = 0;
					rec[1].KeyEvent.UnicodeChar = '\r';  // carriage return
					rec[1].KeyEvent.wRepeatCount = 1;
					rec[1].KeyEvent.wVirtualKeyCode = 0x0d;  // 0x0d = VK_Return
					mvk = MapVirtualKey(0x0d, 0);  // MAPVK_VK_TO_VSC = 0
					rec[1].KeyEvent.wVirtualScanCode = (ushort)mvk;

					int numWritten = 0;
					WriteConsoleInput(Handle, rec, 2, out numWritten);  // output a Return key to the input stream of the console

					FreeConsole();

					return;
				}
			}

			bool createdNew;
			m_Mutex = new Mutex(true, "Grepy2_Mutex", out createdNew);  // check if there is another instance of Grepy already running...
			if (!createdNew)
			{
				Globals.bIsGrepyReadOnly = true;  // ...if so, set the 'readonly' flag to indicate that this instance shouldn't write to the config file (we only want ONE writer)
			}

			Globals.ApplicationPathExe = Assembly.GetExecutingAssembly().Location;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static bool ProcessCommandlineArguments(string[] args)
		{
			string SearchDirectory = "";
			string SearchString = "";

			if( args.Count() == 1 )  // if there's one argument, see if it is a folder name (to handle right clicking on folder in Windows Explorer)
			{
				// We have to "massage" the cmdline argument because of bad '\' parsing in CommandLineToArgvW...
				// http://weblogs.asp.net/jgalloway/archive/2006/10/05/_5B002E00_NET-Gotcha_5D00_-Commandline-args-ending-in-_5C002200_-are-subject-to-CommandLineToArgvW-whackiness.aspx

				// check if the first argument is a search directory...
				SearchDirectory = args[0];

				if( SearchDirectory.StartsWith("\"") )
				{
					SearchDirectory = SearchDirectory.Substring(1, SearchDirectory.Length-1);
				}

				if( SearchDirectory.EndsWith("\"") )
				{
					SearchDirectory = SearchDirectory.Substring(0, SearchDirectory.Length-1);
				}

				if( SearchDirectory.EndsWith(":") )
				{
					SearchDirectory = string.Format("{0}\\", SearchDirectory);
				}

				if( Directory.Exists(SearchDirectory) )
				{
					// store the specified commandline arguments in Globals so that the main form can use them
					Globals.SearchDirectory = Path.GetFullPath(SearchDirectory);;

					return true;
				}

				return false;
			}

			if( args.Count() < 2 )  // if not enough arguments specified, return failed result
			{
				return false;
			}

			bool bRecursiveSpecified = false;
			bool bCaseSensitiveSpecified = false;
			bool bNoRegExSpecified = false;
			bool bParsedFirstArgument = false;
			bool bSearchDirectorySpecified = false;

			List<string> FileSpecs;

			FileSpecs = new List<string>();

			// Commandline format is: "Grepy2.exe [switches] [SearchFolder] SearchString FileSpec [FileSpec...]"
			// The first argument could be a search folder or the search string (possibly surrounded by double quotes).
			// We detect if the first argument is a valid folder name and if so, we assume that is the search folder
			// and will be followed by the search string.

			// process commandline arguments and display SearchForm if needed...
			for( int index = 0; index < args.Count(); index++ )
			{
				if( args[index].ToLower() == "-r" )  // recursive directory search?
				{
					bRecursiveSpecified = true;
					continue;
				}
				if( args[index].ToLower() == "-cs" )  // case sensitive search?
				{
					bCaseSensitiveSpecified = true;
					continue;
				}
				if( args[index].ToLower() == "-noregex" )  // turn off regular expressions?
				{
					bNoRegExSpecified = true;
					continue;
				}

				if( !bParsedFirstArgument )  // have we not parsed the first commandline argument yet?
				{
					bParsedFirstArgument = true;

					// We have to "massage" the cmdline argument because of bad '\' parsing in CommandLineToArgvW...
					// http://weblogs.asp.net/jgalloway/archive/2006/10/05/_5B002E00_NET-Gotcha_5D00_-Commandline-args-ending-in-_5C002200_-are-subject-to-CommandLineToArgvW-whackiness.aspx

					// check if the first argument is a search directory...
					SearchDirectory = args[index];

					if( SearchDirectory.StartsWith("\"") )
					{
						SearchDirectory = SearchDirectory.Substring(1, SearchDirectory.Length-1);
					}

					if( SearchDirectory.EndsWith("\"") )
					{
						SearchDirectory = SearchDirectory.Substring(0, SearchDirectory.Length-1);
					}

					if( SearchDirectory.EndsWith(":") )
					{
						SearchDirectory = string.Format("{0}\\", SearchDirectory);
					}

					if( Directory.Exists(SearchDirectory) )
					{
						bSearchDirectorySpecified = true;  // the first argument was the search directory

						SearchDirectory = Path.GetFullPath(SearchDirectory);
					}
					else
					{
						SearchString = args[index];  // if the first argument is not a valid directory, then it is the search string
						SearchDirectory = "";  // no directory specified, search using the current directory
					}
				}
				else  // anything after the first argument is either the SearchString, or FileSpecs...
				{
					if( SearchString == "" )
					{
						SearchString = args[index];
					}
					else
					{
						FileSpecs.Add(args[index]);
					}
				}
			}

			if( bSearchDirectorySpecified && (SearchString == "") )  // if we specified a SearchDirectory, but no SearchString, return failed result
			{
				return false;
			}
			else if( (SearchString != "") && (FileSpecs.Count() == 0) )  // if we specified a search string but no file specs, return failed result
			{
				return false;
			}

			if( !bNoRegExSpecified )
			{
				try
				{
					Regex SearchRegEx = new Regex(SearchString, RegexOptions.IgnoreCase);
				}
				catch
				{
					CommandlineErrorMessage = string.Format("The SearchString '{0}' is not a valid regular expression.", SearchString);
					return false;
				}
			}

			if( !bSearchDirectorySpecified )  // if we didn't specifiy a search folder, then use the current directory
			{
				SearchDirectory = Directory.GetCurrentDirectory();
			}

			// store the specified commandline arguments in Globals so that the main form can use them
			Globals.SearchDirectory = SearchDirectory;
			Globals.SearchString = SearchString;
			Globals.FileSpecs = FileSpecs;

			Globals.bRecursive = bRecursiveSpecified;
			Globals.bCaseSensitive = bCaseSensitiveSpecified;
			Globals.bRegEx = !bNoRegExSpecified;

			//
			// TODO - Do we want to save the config settings here???
			//

			return true;
		}
	}
}
