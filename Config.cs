using System;
using System.Collections.Generic;
using System.Linq;

using System.Timers;
using System.IO;

namespace Grepy2
{
	static class Config
	{
		public enum KEY  // everthing is 'int' unless otherwise noted
		{
			PosX,
			PosY,
			Width,
			Height,
			Maximized,  // bool
			SplitterType,  // int (0=horizontal, 1=vertical)
			SplitterHorizontalDistance,
			SplitterVerticalDistance,
			PreviewBeforeSlider,
			PreviewAfterSlider,
			ListViewColumnWidth,  // array of int
			NumWorkerThreads,

			OptionsPosX,
			OptionsPosY,
			OptionsFontName,  // string
			OptionsFontSize,  // float
			OptionsFontBold,  // bool
			OptionsFontItalic,  // bool
			OptionsDeferRichTextDisplay,  // bool
			OptionsUseWindowsFileAssociation,  // bool
			OptionsCustomEditor,  // string

			FontPickerPosX,
			FontPickerPosY,

			SearchPosX,
			SearchPosY,
			SearchRegularExpression,  // bool
			SearchMatchCase,  // bool
			SearchRecursive,  // bool
			SearchText,  // array of string
			SearchFileSpec,  // array of string
			SearchFolder,  // array of string

			SearchHelpPosX,
			SearchHelpPosY,

			DisplayLineNumbers,  // bool
		};

		private static string ConfigFilename;
		private static Dictionary<string, string> ConfigDictionary;
		private static Timer timer;

		static Config()
		{
			string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string AppDataFolder = AppDataPath + "\\Grepy2";

			Directory.CreateDirectory(AppDataFolder);

			ConfigFilename = AppDataPath + "\\Grepy2\\Grepy2.ini";

			ConfigDictionary = new Dictionary<string,string>();

			Load();

			timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(OnTimerEvent);
			timer.AutoReset = false;
			timer.Enabled = false;
		}

		private static void OnTimerEvent(object source, ElapsedEventArgs e)
		{
			Save();
		}

		private static void Load()
		{
			string line;

			try
			{
				using( StreamReader sr = new StreamReader(ConfigFilename) )
				{
					while (sr.Peek() >= 0)
					{
						line = sr.ReadLine();

						while( (line.Length > 0) && ((line[0] == ' ') || (line[0] == '\t')) )  // remove leading whitespace
						{
							line = line.Substring(1, line.Length-1);
						}

						int end = line.Length-1;
						while( (line.Length > 0) && ((line[end] == ' ') || (line[end] == '\t')) )  // remove trailing whitespace
						{
							if( end > 0 )
								line = line.Substring(0, end);
							else
								line = "";

							end = line.Length-1;
						}

						if( (line.Length > 0) && (line[0] != '[') )  // not a blank line or a section?
						{
							int pos = line.IndexOf("=");

							if( (pos > 0) && (pos < (line.Length-1)) )  // '=' must not be the first or last character of the line
							{
								string key = line.Substring(0, pos);
								string value = line.Substring(pos+1, (line.Length-pos)-1);

								ConfigDictionary.Add(key, value);
							}
						}
					}
				}
			}
			catch( Exception )
			{
				Console.WriteLine("Config() - can't read config file: '{0}'", ConfigFilename);
			}
		}

		public static void Save()
		{
			timer.Enabled = false;  // turn off the timer (in the event that Save was called directly)

			if( !Globals.bIsGrepyReadOnly )
			{
				try
				{
					using( StreamWriter sw = new StreamWriter(ConfigFilename) )
					{
						sw.WriteLine("[Grepy2]");

						foreach( KEY key in Enum.GetValues(typeof(KEY)) )
						{
							if( key == KEY.ListViewColumnWidth ||
								key == KEY.SearchText ||
								key == KEY.SearchFileSpec ||
								key == KEY.SearchFolder )
							{
								int count = 1;
								bool found = false;
								do
								{
									string key_string = string.Format("{0}{1}", key.ToString(), count);
									found = ConfigDictionary.ContainsKey(key_string);
									if( found )
									{
										sw.WriteLine(string.Format("{0}={1}", key_string, ConfigDictionary[key_string]));
									}
									count++;
								}  while( found );
							}
							else
							{
								string key_string = key.ToString();
								if( ConfigDictionary.ContainsKey(key_string) )
								{
									sw.WriteLine(string.Format("{0}={1}", key_string, ConfigDictionary[key_string]));
								}
							}
						}
					}
				}
				catch( Exception )
				{
					Console.WriteLine("SaveConfig() - Error saving config file!");
				}
			}
		}

		public static void Set(KEY key, string value)
		{
			string key_string = key.ToString();

			if( ConfigDictionary.ContainsKey(key_string) )  // if the key/value pair exists...
			{
				ConfigDictionary.Remove(key_string);  // ...remove the old key/value pair
			}

			ConfigDictionary.Add(key_string, value);  // add the key/value pair to the dictionary

			timer.Enabled = true;
			timer.Interval = 1000;
		}

		public static void Set(KEY key, int value)
		{
			Set(key, value.ToString());
		}

		public static void Set(KEY key, bool value)
		{
			Set(key, value.ToString());  // set to 'False' or 'True'
		}

		public static void Set(KEY key, List<string> StringList)
		{
			int count = 1;
			bool found = false;
			// remove ALL old key/value pairs from the dictionary (since the list parameter passed in can be different size than what's currently in dictionary)
			do
			{
				string key_string = string.Format("{0}{1}", key.ToString(), count);
				found = ConfigDictionary.ContainsKey(key_string);
				if( found )
				{
					ConfigDictionary.Remove(key_string);
				}
				count++;
			}  while( found );

			// add the new key/value pairs to the dictionary
			for( int index = 0; index < StringList.Count; index++ )
			{
				string key_string = string.Format("{0}{1}", key.ToString(), index+1);
				ConfigDictionary.Add(key_string, StringList[index]);
			}

			timer.Enabled = true;
			timer.Interval = 1000;
		}

		public static void Set(KEY key, List<int> IntList)
		{
			List<string> StringList = new List<string>();

			for( int index = 0; index < IntList.Count; index++ )
			{
				StringList.Add(IntList[index].ToString());
			}

			Set(key, StringList);
		}

		public static bool Get(KEY key, ref string value)
		{
			string key_string = key.ToString();
			if( ConfigDictionary.ContainsKey(key_string) )
			{
				value = ConfigDictionary[key_string];
				return true;
			}
			return false;
		}

		public static bool Get(KEY key, ref int value)
		{
			string key_string = key.ToString();
			if( ConfigDictionary.ContainsKey(key_string) )
			{
				value = 0;
				Int32.TryParse(ConfigDictionary[key_string], out value);
				return true;
			}
			return false;
		}

		public static bool Get(KEY key, ref bool value)
		{
			string key_string = key.ToString();
			if( ConfigDictionary.ContainsKey(key_string) )
			{
				value = ConfigDictionary[key_string] == "True";
				return true;
			}
			return false;
		}

		public static bool Get(KEY key, ref List<string> value)
		{
			value = new List<string>();

			int count = 1;
			bool found = false;
			do
			{
				string key_string = string.Format("{0}{1}", key.ToString(), count);
				found = ConfigDictionary.ContainsKey(key_string);
				if( found )
				{
					value.Add(ConfigDictionary[key_string]);
				}
				count++;
			}  while( found );

			return (value.Count() > 0);
		}

		public static bool Get(KEY key, ref List<int> value)
		{
			value = new List<int>();

			List<string> StringList = new List<string>();

			Get(key, ref StringList);

			for( int index = 0; index < StringList.Count; index++ )
			{
				value.Add(Convert.ToInt32(StringList[index]));
			}

			return (value.Count() > 0);
		}
	}
}
