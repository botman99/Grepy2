using System;

using System.Windows.Forms;
using System.Reflection;

namespace Grepy2
{
	// based on https://www.lyquidity.com/devblog/?p=136

	class FolderSelectDialog
	{
		OpenFileDialog OpenFileDir = null;

		string name_string;
		Assembly assembly;

		public FolderSelectDialog()
		{
			OpenFileDir = new OpenFileDialog();

			OpenFileDir.Filter = "Folders|\n";

			OpenFileDir.Multiselect = false;
			OpenFileDir.AddExtension = false;
			OpenFileDir.CheckFileExists = false;
			OpenFileDir.DereferenceLinks = true;
		}

		public string InitialDirectory
		{
			get { return OpenFileDir.InitialDirectory; }
			set
			{
				bool bIsNull = ((value == null) || (value.Length == 0));
				OpenFileDir.InitialDirectory = bIsNull ? Environment.CurrentDirectory : value;
			}
		}

		public string Title
		{
			get { return OpenFileDir.Title; }
			set
			{
				OpenFileDir.Title = (value == null) ? "Select a folder" : value;
			}
		}

		public string FileName
		{
			get { return OpenFileDir.FileName; }
		}

		public bool ShowDialog()
		{
			return ShowDialog(IntPtr.Zero);
		}

		public bool ShowDialog(IntPtr hWndOwner)
		{
			if( Environment.OSVersion.Version.Major >= 6 )  // Vista/Win7/Win8/Win10
			{
				name_string = "System.Windows.Forms";
				assembly = null;
				AssemblyName[] referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
				foreach (AssemblyName assemblyName in referencedAssemblies)
				{
					if (assemblyName.FullName.StartsWith(name_string))
					{
						assembly = Assembly.Load(assemblyName);
						break;
					}
				}

				Type typeIFileDialog = GetType("FileDialogNative.IFileDialog");
				object dialog = Call(OpenFileDir.GetType(), OpenFileDir, "CreateVistaDialog");
				Call(OpenFileDir.GetType(), OpenFileDir, "OnBeforeVistaDialog", dialog);

				uint options = (uint)Call(typeof(System.Windows.Forms.FileDialog), OpenFileDir, "GetOptions");
				options = options | (uint)GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
				Call(typeIFileDialog, dialog, "SetOptions", options);

				object pFileDialogEvent = New("FileDialog.VistaDialogEvents", OpenFileDir);

				uint num_parms = 0;

				object[] parameters = new object[] { pFileDialogEvent, num_parms };
				Call(typeIFileDialog, dialog, "Advise", parameters);

				num_parms = (uint)parameters[1];

				try
				{
					// show the dialog
					int count = (int)Call(typeIFileDialog, dialog, "Show", hWndOwner);
					return (count == 0);
				}
				finally
				{
					// remove event handler
					Call(typeIFileDialog, dialog, "Unadvise", num_parms);
					GC.KeepAlive(pFileDialogEvent);
				}
			}
			else  // XP and earlier
			{
				FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();

				FolderBrowser.Description = this.Title;
				FolderBrowser.SelectedPath = this.InitialDirectory;
				FolderBrowser.ShowNewFolderButton = false;

				DialogResult dialog_result = FolderBrowser.ShowDialog();
				if( dialog_result == DialogResult.OK )
				{
					OpenFileDir.FileName = FolderBrowser.SelectedPath;
					return true;
				}
			}
			return false;
		}

		public Type GetType(string typeName)
		{
			Type type = null;
			string[] type_names = typeName.Split('.');

			if (type_names.Length > 0)
			{
				type = assembly.GetType(name_string + "." + type_names[0]);
			}

			for (int i = 1; i < type_names.Length; ++i)
			{
				type = type.GetNestedType(type_names[i], BindingFlags.NonPublic);
			}

			return type;
		}

		public object GetEnum(string typeName, string name)
		{
			Type type = GetType(typeName);
			FieldInfo fieldInfo = type.GetField(name);
			return fieldInfo.GetValue(null);
		}

		public object Call(Type type, object obj, string func, params object[] parameters)
		{
			MethodInfo methodInfo = type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return methodInfo.Invoke(obj, parameters);
		}

		public object New(string name, params object[] parameters)
		{
			Type type = GetType(name);

			ConstructorInfo[] constructorInfos = type.GetConstructors();
			foreach (ConstructorInfo constructorInfo in constructorInfos)
			{
				try
				{
					return constructorInfo.Invoke(parameters);
				}
				catch
				{
					Console.WriteLine("constructorInfo.Invoke() failed");
				}
			}

			return null;
		}
	}
}
