# X-Mouse Setting Tool

X-Mouse setting tool for Windows8Windows7/Vista/XP (with .NET 3.5).

## Requirement

- Windows8/Windows7/Vista/XP (with .NET 3.5): not required .NET SDK

## Build

Build `XMouseSetting.exe` on command line

    csc.exe /t:winexe XMouseSetting.cs /r:PresentationFramework.dll /r:PresentationCore.dll /r:WindowsBase.dll /win32icon:icon.ico

## Binary

- [github downloads](https://github.com/bellbind/XMouseSetting/downloads/)

## Usage: GUI

- Launch `XMouseSetting.exe`. 
- Change settings and push "save" button, then logoff. 
- Login again.

## Usage: Command-line

Print help and current settings:

    XMouseSetting.exe help

Set to activate a window over mouse:

    XMouseSetting.exe on

Set to activate a window over mouse then to raise after 500 msec:

    XMouseSetting.exe on 500

Off X-Mouse settings (Click to activate and raise):

    XMouseSetting.exe off

NOTICE: To enable new settings, you must **re-login**.

## Other

- Author: [http://twitter.com/bellbind](http://twitter.com/bellbind)
- Site: [http://github.com/bellbind/XMouseSetting](http://github.com/bellbind/XMouseSetting)
- License: [LGPL](http://www.opensource.org/licenses/lgpl-3.0.html)

## Hints

### Location of `csc.exe` 

On 64bit Windows .NET 3.5:

    c:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe

On 32bit Windows .NET 3.5:

    c:\Windows\Microsoft.NET\Framework\v3.5\csc.exe


