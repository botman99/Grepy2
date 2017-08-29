using System;

using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Grepy2
{
	public class ExplorerIntegration
	{
		/// <summary>
		/// Passed to <see cref="GetTokenInformation"/> to specify what
		/// information about the token to return.
		/// </summary>
		enum TokenInformationClass
		{
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin,
			TokenElevationType,
			TokenLinkedToken,
			TokenElevation,
			TokenHasRestrictions,
			TokenAccessInformation,
			TokenVirtualizationAllowed,
			TokenVirtualizationEnabled,
			TokenIntegrityLevel,
			TokenUiAccess,
			TokenMandatoryPolicy,
			TokenLogonSid,
			MaxTokenInfoClass
		}

		/// <summary>
		/// The elevation type for a user token.
		/// </summary>
		enum TokenElevationType
		{
			TokenElevationTypeDefault = 1,
			TokenElevationTypeFull,
			TokenElevationTypeLimited
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		static extern bool GetTokenInformation(IntPtr tokenHandle, TokenInformationClass tokenInformationClass, IntPtr tokenInformation, int tokenInformationLength, out int returnLength);

		bool bIsAdministrator;

		private bool IsUserAdministrator()
		{
			// http://www.davidmoore.info/blog/2011/06/20/how-to-check-if-the-current-user-is-an-administrator-even-if-uac-is-on/

			var identity = WindowsIdentity.GetCurrent();

			if (identity == null)
			{
				throw new InvalidOperationException("Couldn't get the current user identity");
			}

			var principal = new WindowsPrincipal(identity);

			// Check if this user has the Administrator role. If they do, return immediately.
			// If UAC is on, and the process is not elevated, then this will actually return false.
			if (principal.IsInRole(WindowsBuiltInRole.Administrator))
			{
				return true;
			}

			// If we're not running in Vista onwards, we don't have to worry about checking for UAC.
			if (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version.Major < 6)
			{
				// Operating system does not support UAC; skipping elevation check.
				return false;
			}

			int tokenInfLength = Marshal.SizeOf(typeof(int));
			IntPtr tokenInformation = Marshal.AllocHGlobal(tokenInfLength);

			try
			{
				var token = identity.Token;
				var result = GetTokenInformation(token, TokenInformationClass.TokenElevationType, tokenInformation, tokenInfLength, out tokenInfLength);

				if (!result)
				{
					var exception = Marshal.GetExceptionForHR( Marshal.GetHRForLastWin32Error() );
					throw new InvalidOperationException("Couldn't get token information", exception);
				}

				var elevationType = (TokenElevationType)Marshal.ReadInt32(tokenInformation);
    
				switch (elevationType)
				{
					case TokenElevationType.TokenElevationTypeDefault:
						// TokenElevationTypeDefault - User is not using a split token, so they cannot elevate.
						return false;

					case TokenElevationType.TokenElevationTypeFull:
						// TokenElevationTypeFull - User has a split token, and the process is running elevated. Assuming they're an administrator.
						return true;

					case TokenElevationType.TokenElevationTypeLimited:
						// TokenElevationTypeLimited - User has a split token, but the process is not running elevated. Assuming they're an administrator.
						return true;

					default:
						// Unknown token elevation type.
						return false;
				}
			}
			finally
			{
				if (tokenInformation != IntPtr.Zero) Marshal.FreeHGlobal(tokenInformation);
			}
		}

		public bool IsEnabled(string where)
		{
			if( where == "here" )
			{
				string regPath = string.Format(@"SOFTWARE\Classes\Directory\Background\shell\Grepy2");
				try
				{
					return Registry.LocalMachine.OpenSubKey(regPath) != null;
				}
				catch( UnauthorizedAccessException )
				{
					return false;
				}
			}
			else
			{
				string regPath = string.Format(@"{0}\shell\Grepy2", where);
				try
				{
					return Registry.ClassesRoot.OpenSubKey(regPath) != null;
				}
				catch( UnauthorizedAccessException )
				{
					return false;
				}
			}
		}

		public void Enable(string where, string ApplicationExePath)
		{
			try
			{
				if( where == "here" )
				{
					string regPath = string.Format(@"SOFTWARE\Classes\Directory\Background\shell\Grepy2");

					RegistryKey keyName = Registry.LocalMachine.CreateSubKey(regPath);
					keyName.SetValue(null, "Grepy2...");
					RegistryKey keyIcon = Registry.LocalMachine.CreateSubKey(string.Format(@"{0}", regPath));
					keyIcon.SetValue("Icon", ApplicationExePath);

					string command = string.Format("\"{0}\" \"%V\"", ApplicationExePath);

					RegistryKey keyCommand = Registry.LocalMachine.CreateSubKey(string.Format(@"{0}\command", regPath));
					keyCommand.SetValue(null, command);
				}
				else
				{
					string regPath = string.Format(@"{0}\shell\Grepy2", where);

					RegistryKey keyName = Registry.ClassesRoot.CreateSubKey(regPath);
					keyName.SetValue(null, "Grepy2...");

					RegistryKey keyIcon = Registry.ClassesRoot.CreateSubKey(string.Format(@"{0}", regPath));
					keyIcon.SetValue("Icon", ApplicationExePath);

					string command = string.Format("\"{0}\" \"%1\"", ApplicationExePath);

					RegistryKey keyCommand = Registry.ClassesRoot.CreateSubKey(string.Format(@"{0}\command", regPath));
					keyCommand.SetValue(null, command);
				}
			}
			catch
			{
				bIsAdministrator = false;
			}
		}
	
		public void Disable(string where, string ApplicationExePath)
		{
			try
			{
				if( where == "here" )
				{
					string regPath = string.Format(@"SOFTWARE\Classes\Directory\Background\shell\Grepy2");
					Registry.LocalMachine.DeleteSubKeyTree(regPath);
				}
				else
				{
					string regPath = string.Format(@"{0}\shell\Grepy2", where);
					Registry.ClassesRoot.DeleteSubKeyTree(regPath);
				}
			}
			catch
			{
				bIsAdministrator = false;
			}
		}

		public void Set(string PathExe, bool bEnable)
		{
			bIsAdministrator = IsUserAdministrator();

			if( bIsAdministrator )
			{
				if( bEnable )
				{
					Enable("Drive", PathExe);  // right clicking on a Disk (drive, like C:)
					Enable("Directory", PathExe);  // right clicking on a folder (this can be a folder in the navigation pane on the left or in the file pane on the right)
					Enable("here", PathExe);  // right clicking on the empty part of a folder in the file pane (i.e. use the folder I am clicking inside of)
				}
				else
				{
					Disable("Drive", PathExe);
					Disable("Directory", PathExe);
					Disable("here", PathExe);  // right clicking on the empty part of a folder
				}
			}

			if( !bIsAdministrator )
			{
				MessageBox.Show("You need to run this program with Administrator Privileges to change Windows Explorer Integration.\n\n(Right click on the Grepy icon in the Start menu or on the desktop shortcut and select \"Run as administrator\" or \"More -> Run as administrator\".)",
					"Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}
	}
}
