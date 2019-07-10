using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace Grepy2
{
	public class Worker
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		const int BUFFER_SIZE_4K = 4096;
		byte[] Buffer_4k;  // buffer used to read 4K of data from file to determine if file is binary (not text)

		const int MAX_LINE_LENGTH = 65536;  // the maximum supported length of a line of text read from a file (this is limited for performance reasons)

		private static HandleRef FormHandle;
		private int MyIndex;
		private IntPtr MyIndexPtr;

		int ReadLineSleepCount = 0;

		StringBuilder currentline;  // for ReadLine method

		int SearchFilesIndex;  // the index into the Globals.SearchFiles[] array that this worker is currently working on

		private class MatchPos
		{
			public int StartPos;
			public int Length;

			public MatchPos(int InStartPos, int InLength)
			{
				StartPos = InStartPos;
				Length = InLength;
			}
		}

		public Worker(IntPtr InHandle, int InWorkerIndex)
		{
			if( Globals.Workers != null && (InWorkerIndex < Globals.Workers.Count()) )
			{
				FormHandle = new HandleRef(this, InHandle);
				MyIndex = InWorkerIndex;
				MyIndexPtr = new IntPtr(MyIndex);

				Globals.Workers[MyIndex].WaitHandle = new AutoResetEvent(false);  // create a WaitHandle for syncronization

				Globals.Workers[MyIndex].bIsWorking = false;
				Globals.Workers[MyIndex].bShouldExit = false;

				Buffer_4k = new byte[BUFFER_SIZE_4K];

				currentline = new StringBuilder(MAX_LINE_LENGTH);  // maximum input line length is 64K
			}
		}

		public void Run()
		{
			while( !Globals.Workers[MyIndex].bShouldExit )
			{
				Globals.Workers[MyIndex].WaitHandle.WaitOne();  // wait for something to do

				Globals.Workers[MyIndex].bIsWorking = true;

				SearchFilesIndex = Globals.Workers[MyIndex].SearchFilesIndex;

				if( !File.Exists(Globals.SearchFiles[SearchFilesIndex].Filename) )  // verify that the file exists
				{
					Globals.SearchFiles[SearchFilesIndex].Status = Globals.SearchFileStatus.SearchTextNotFound;
				}
				else
				{
					if( IsFileBinary(Globals.SearchFiles[SearchFilesIndex].Filename) )  // check if the file is binary
					{
						Globals.SearchFiles[SearchFilesIndex].Status = Globals.SearchFileStatus.FileIsBinary;
					}
					else
					{
						if( !ScanFileForMatches(Globals.SearchFiles[SearchFilesIndex].Filename) )
						{
							Globals.SearchFiles[SearchFilesIndex].Status = Globals.SearchFileStatus.SearchTextNotFound;
						}
						else
						{
							Globals.SearchFiles[SearchFilesIndex].Status = Globals.SearchFileStatus.SearchTextFound;
						}
					}
				}

				if( !Globals.Workers[MyIndex].bShouldExit )
				{
					Globals.Workers[MyIndex].bIsWorking = false;

					// notify main Form thread that we are done (or that we were cancelled)
					PostMessage(FormHandle, Globals.WM_WORKER_NOTIFY_MAIN_THREAD, MyIndexPtr, IntPtr.Zero);
				}

				Thread.Sleep(0);  // force context switch
			}
		}

		private bool IsFileBinary(string Filename)  // check if this file is a binary file (not a text file)
		{
			try
			{
				int buffer_size = BUFFER_SIZE_4K;

				FileInfo fInfo = new FileInfo( Filename );
				if( fInfo.Length < buffer_size )
				{
					buffer_size = (int)fInfo.Length;
				}

				int count = 0;
				using (FileStream readStream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					count = readStream.Read(Buffer_4k, 0, buffer_size);
				}

				for( int i = 3; i < count-1; i++ )  // skip the first 3 bytes since these are the Unicode BOM encoding
				{
					if( Globals.bShouldStopWorkerJobs )
					{
						return false;
					}

					if( (Buffer_4k[i] == 0) && (Buffer_4k[i+1] == 0) )
					{
						return true; // two zero bytes in a row indicate the file is binary
					}
				}

				return false;
			}
			catch( Exception e )
			{
				Console.WriteLine("IsFileBinary() Exception: {0}", e.Message);
				return false;
			}
		}

		private string ReadLine(StreamReader sr)  // read a line of text from the input stream (we use this instead of StreamReader.ReadLine() so we can cancel a search easier)
		{
			int i;

			currentline.Clear();

			while( (i = sr.Read()) > 0 )
			{
				if( Globals.bShouldStopWorkerJobs )
				{
					sr.Close();
					return "";
				}

				if( i == 13 )  // if character is a carriage return
				{
					int next_char = sr.Peek();

					if( next_char == 10 )  // if CR followed by LF
					{
						sr.Read();  // throw away the newline
					}

					break;
				}
				else if( i == 10 )
				{
					break;
				}

				char c = (char)i;
				currentline.Append(c);

				if( currentline.Length == MAX_LINE_LENGTH )
				{
					while( (i = sr.Read()) > 0 )  // read until we get an end of line character
					{
						if( Globals.bShouldStopWorkerJobs )
						{
							sr.Close();
							return "";
						}

						if( i == 13 )  // if character is a carriage return
						{
							int next_char = sr.Peek();

							if( next_char == 10 )  // if CR followed by LF
							{
								sr.Read();  // throw away the newline
							}

							break;
						}
						else if( i == 10 )
						{
							break;
						}
					}

					break;
				}
			}

			return currentline.ToString();
		}

		// add a line of text that contains search text match(es) to the 'Lines' list in the SearchFile
		private void AddMatchLine(ref string line, ref List<MatchPos> Matches, ref string[] PreviewLines, ref int PreviewLineIndex, ref int PreviewLineCount, ref int LineNumber, ref int LinesSinceMatch)
		{
			if( Globals.SearchFiles[SearchFilesIndex].Lines == null )
			{
				Globals.SearchFiles[SearchFilesIndex].Lines = new List<Globals.SearchLine>();
			}

			if( PreviewLineCount > 0 )
			{
				int PreviewIndex = PreviewLineIndex - (PreviewLineCount - 1);  // index in the circular buffer of the oldest preview line
				if( PreviewIndex < 0 )
				{
					PreviewIndex += 5;
				}

				for( int index = 0; index < PreviewLineCount; index++ )
				{
					Globals.SearchLine PreviewSearchLine = new Globals.SearchLine();

					PreviewSearchLine.LineNumber = LineNumber - (PreviewLineCount - index);
					PreviewSearchLine.Line = PreviewLines[PreviewIndex++];
					if( PreviewIndex == 5 )
					{
						PreviewIndex = 0;
					}
					PreviewSearchLine.bIsSearchTextMatch = false;
					PreviewSearchLine.LinesBeforeNextMatch = -1;

					Globals.SearchFiles[SearchFilesIndex].Lines.Add(PreviewSearchLine);
				}
			}

			Globals.SearchLine SearchLine = new Globals.SearchLine();

			SearchLine.LineNumber = LineNumber;
			SearchLine.Line = line;
			SearchLine.bIsSearchTextMatch = true;
			SearchLine.SearchMatchPositions = new List<Globals.SearchMatchPosition>();
			SearchLine.LinesBeforeNextMatch = 0;

			for( int index = 0; index < Matches.Count; index++ )
			{
				Globals.SearchMatchPosition SearchMatchPosition = new Globals.SearchMatchPosition();

				SearchMatchPosition.StartPos = Matches[index].StartPos;
				SearchMatchPosition.Length = Matches[index].Length;

				SearchLine.SearchMatchPositions.Add(SearchMatchPosition);
			}

			Globals.SearchFiles[SearchFilesIndex].Lines.Add(SearchLine);
			Globals.SearchFiles[SearchFilesIndex].Matches += Matches.Count;

			// go back through (up to 5) previous Lines to indicate their position relative to this match
			int CurrentLineIndex = Globals.SearchFiles[SearchFilesIndex].Lines.Count - 1;
			for( int index = 1; (index <= 5) && ((CurrentLineIndex - index) >= 0); index++ )
			{
				if( Globals.SearchFiles[SearchFilesIndex].Lines[CurrentLineIndex - index].bIsSearchTextMatch )
				{
					break;
				}

				Globals.SearchFiles[SearchFilesIndex].Lines[CurrentLineIndex - index].LinesBeforeNextMatch = index;
			}

			PreviewLineIndex = -1;  // reset the circular buffer
			PreviewLineCount = 0;

			LinesSinceMatch = 0;
		}

		private bool ScanFileForMatches(string Filename)  // scan through the specified file to look for search text matches
		{
			Regex regex = null;

			if( Globals.bRegEx )
			{
				regex = new Regex(Globals.SearchString, Globals.bCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
			}

			StreamReader sr = null;

			try
			{
				sr = new StreamReader(Filename,true);
			}
			catch( Exception e )
			{
				Console.WriteLine("ScanFileForMatches() Exception: {0}", e.Message);
				return false;
			}

			bool bFoundMatchInFile = false;
			int LineNumber = 0;
			int NumberOfMatchesInFile = 0;

			string[] PreviewLines = new string[5];  // circular buffer of preview lines (for displaying before and after matches)
			int PreviewLineIndex = -1;  // we haven't added a line to the circular buffer yet
			int PreviewLineCount = 0;  // number of lines in the preview circular buffer that are valid

			int LinesSinceMatch = -1;  // reset this to 0 when we find a match so we know when to store "after" preview lines following a match

			string line = "";

			while (!Globals.bShouldStopWorkerJobs && (sr.Peek() >= 0))
			{
				line = ReadLine(sr);

				LineNumber++;

				ReadLineSleepCount++;
				if( ReadLineSleepCount >= 1000 )
				{
					ReadLineSleepCount = 0;
					Thread.Sleep(0);  // force context switch
				}

				int MatchesInLine = 0;

				if( Globals.bRegEx && (regex != null) )
				{
					MatchCollection matches = regex.Matches(line);
					MatchesInLine = matches.Count;

					if( matches.Count > 0 )
					{
						List<MatchPos> Matches = new List<MatchPos>();

						for( int i = 0; i < matches.Count; i++ )
						{
							MatchPos Match = new MatchPos(matches[i].Index, matches[i].Length);
							Matches.Add(Match);
						}

						AddMatchLine(ref line, ref Matches, ref PreviewLines, ref PreviewLineIndex, ref PreviewLineCount, ref LineNumber, ref LinesSinceMatch);
					}
				}
				else
				{
					int pos = line.IndexOf(Globals.SearchString, 0, Globals.bCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

					if( pos >= 0 )
					{
						List<MatchPos> Matches = new List<MatchPos>();

						while( pos >= 0 )
						{
							if( Globals.bShouldStopWorkerJobs )
							{
								sr.Close();
								return false;
							}

							MatchesInLine++;

							MatchPos Match = new MatchPos(pos, Globals.SearchString.Length);
							Matches.Add(Match);

							if( pos < (line.Length - 1) )
							{
								pos = line.IndexOf(Globals.SearchString, pos+1, Globals.bCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
							}
							else
							{
								pos = -1;
							}
						}

						AddMatchLine(ref line, ref Matches, ref PreviewLines, ref PreviewLineIndex, ref PreviewLineCount, ref LineNumber, ref LinesSinceMatch);
					}
				}

				if( MatchesInLine > 0 )
				{
					bFoundMatchInFile = true;
				}
				else  // if no matches, add this line to the circular preview buffer
				{
					PreviewLineIndex = (PreviewLineIndex + 1) % 5;
					PreviewLines[PreviewLineIndex] = line;

					PreviewLineCount = Math.Min(PreviewLineCount + 1, 5);

					if( LinesSinceMatch >= 0 )
					{
						LinesSinceMatch++;

						try
						{
							if( (LinesSinceMatch == 5) || (sr.Peek() == -1) )  // if we're read 5 lines or reached the end of the file...
							{
								int PreviewIndex = PreviewLineIndex - (PreviewLineCount - 1);  // index in the circular buffer of the oldest preview line
								if( PreviewIndex < 0 )
								{
									PreviewIndex += 5;
								}

								for( int index = 0; index < PreviewLineCount; index++ )
								{
									Globals.SearchLine PreviewSearchLine = new Globals.SearchLine();

									PreviewSearchLine.LineNumber = LineNumber - (PreviewLineCount - index) + 1;
									PreviewSearchLine.Line = PreviewLines[PreviewIndex++];
									if( PreviewIndex == 5 )
									{
										PreviewIndex = 0;
									}
									PreviewSearchLine.bIsSearchTextMatch = false;

									Globals.SearchFiles[SearchFilesIndex].Lines.Add(PreviewSearchLine);
								}

								PreviewLineIndex = -1;  // reset the circular buffer
								PreviewLineCount = 0;

								LinesSinceMatch = -1;
							}
						}
						catch( Exception e )
						{
							Console.WriteLine("ScanFileForMatches() Exception: {0}", e.Message);
						}
					}
				}

				NumberOfMatchesInFile += MatchesInLine;
			}

			try
			{
				sr.Close();
			}
			catch( Exception e )
			{
				Console.WriteLine("ScanFileForMatches() Exception: {0}", e.Message);
			}

			if (Globals.bShouldStopWorkerJobs)
			{
				return false;
			}

			Globals.SearchFiles[SearchFilesIndex].SearchMatchCount = NumberOfMatchesInFile;

			return bFoundMatchInFile;
		}

	}
}
