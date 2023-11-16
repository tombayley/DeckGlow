#define AppName "Deck Glow"
#define AppExe "DeckGlow.exe"
#define AppVersion "1.0.1"

[Setup]
AppId={{A83104B4-81A8-4EC9-8619-FE97829D5C26}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher="Tom Bayley"
AppPublisherURL="https://github.com/tombayley/DeckGlow"
AppSupportURL="https://github.com/tombayley/DeckGlow/issues"
AppUpdatesURL="https://github.com/tombayley/DeckGlow/releases"
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
SetupIconFile=..\DeckGlow\Assets\app-icon.ico
OutputDir=bin\
OutputBaseFilename=DeckGlow-Installer
UninstallDisplayIcon={app}\{#AppExe}
WizardStyle=modern

[Files]
Source: "Source\*"; DestDir: "{app}";

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{group}\{#AppName} on Github"; Filename: "https://github.com/tombayley/DeckGlow"

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "DeckGlow"; ValueData: "{app}\{#AppExe}"

[Run]
Filename: "{app}\{#AppExe}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Name: "{userappdata}\DeckGlow"; Type: filesandordirs
