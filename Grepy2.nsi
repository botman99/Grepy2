; Grepy2.nsi
;
; This script is based on example2.nsi, but it remembers the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
;--------------------------------

; The name of the installer
Name "Grepy2"

!define VERSION '2.0.0'

; The file to write
OutFile "Grepy2-${VERSION}.exe"

; The default installation directory
InstallDir $PROGRAMFILES\Grepy2

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Grepy2" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "Grepy2"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File "bin\x86\Release\Grepy2.exe"
  File "Everything32.dll"
  File "Grepy2Help\Grepy2Help.chm"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Grepy2 "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "DisplayName" "Grepy2"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "DisplayIcon" "$INSTDIR\Grepy2.exe,0"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "HelpLink" '"$INSTDIR\Grepy2Help.chm"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "EstimatedSize" 800
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "Publisher" "Jeffrey Broome"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"
  
  CreateDirectory "$SMPROGRAMS\Grepy2"
  CreateShortCut "$SMPROGRAMS\Grepy2\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Grepy2\Grepy2.lnk" "$INSTDIR\Grepy2.exe" "" "$INSTDIR\Grepy2.exe" 0
  CreateShortCut "$SMPROGRAMS\Grepy2\Grepy2 Help.lnk" "$INSTDIR\Grepy2Help.chm" "" "$INSTDIR\Grepy2Help.chm" 0
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Desktop Shortcut"
  
  CreateShortCut "$DESKTOP\Grepy2.lnk" "$INSTDIR\Grepy2.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Enable Windows File Explorer Integration"
  
  ; Write the installation path into the registry
  WriteRegStr HKCR Drive\shell\Grepy2 "" "Grepy2..."
  WriteRegStr HKCR Drive\shell\Grepy2 "Icon" '"$INSTDIR\Grepy2.exe"'
  WriteRegStr HKCR Drive\shell\Grepy2\command "" '"$INSTDIR\Grepy2.exe" "%1"'
  WriteRegStr HKCR Directory\shell\Grepy2 "" "Grepy2..."
  WriteRegStr HKCR Directory\shell\Grepy2 "Icon" '"$INSTDIR\Grepy2.exe"'
  WriteRegStr HKCR Directory\shell\Grepy2\command "" '"$INSTDIR\Grepy2.exe" "%1"'
  
  WriteRegStr HKLM SOFTWARE\Classes\Directory\Background\shell\Grepy2 "" "Grepy2..."
  WriteRegStr HKLM SOFTWARE\Classes\Directory\Background\shell\Grepy2 "Icon" '"$INSTDIR\Grepy2.exe"'
  WriteRegStr HKLM SOFTWARE\Classes\Directory\Background\shell\Grepy2\command "" '"$INSTDIR\Grepy2.exe" "%V"'

  System::Call 'Shell32::SHChangeNotify(i 0x8000000, i 0, i 0, i 0)'

SectionEnd


;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Grepy2"
  DeleteRegKey HKLM SOFTWARE\Grepy2
  
  DeleteRegKey HKCR Drive\shell\Grepy2\command
  DeleteRegKey HKCR Drive\shell\Grepy2
  DeleteRegKey HKCR Directory\shell\Grepy2\command
  DeleteRegKey HKCR Directory\shell\Grepy2
  DeleteRegKey HKCR *\shell\Grepy2\command
  DeleteRegKey HKCR *\shell\Grepy2
  
  DeleteRegKey HKLM SOFTWARE\Classes\Directory\Background\shell\Grepy2\command
  DeleteRegKey HKLM SOFTWARE\Classes\Directory\Background\shell\Grepy2
  
  System::Call 'Shell32::SHChangeNotify(i 0x8000000, i 0, i 0, i 0)'

  ; Remove shortcuts, if any
  Delete $DESKTOP\Grepy2.lnk
  Delete "$SMPROGRAMS\Grepy2\*.*"
  
  ; Remove files and uninstaller
  Delete $INSTDIR\Grepy2.exe
  Delete $INSTDIR\Everything32.dll
  Delete $INSTDIR\Grepy2Help.chm
  Delete $INSTDIR\uninstall.exe

  IfFileExists $APPDATA\Grepy2\Grepy2.ini Remove_Ini Uninstall_Continue
    Remove_Ini:
    MessageBox MB_YESNO "Do you wish to remove the Grepy2.ini file?" IDNO Uninstall_Continue
    Delete "$APPDATA\Grepy2\Grepy2.ini"
	RMDir "$APPDATA\Grepy2"
  Uninstall_Continue:
  
  ; Remove directories used
  RMDir "$SMPROGRAMS\Grepy2"
  RMDir "$INSTDIR"
  
SectionEnd
