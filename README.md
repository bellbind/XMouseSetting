# X-Mouse Setting Tool

X-Mouse setting tool for Windows7/Vista.

## Requirement

- Windows7/Vista (with .NET 3.5)

## Build

Build `XMouseSetting.exe` on command line

    csc.exe /t:winexe XMouseSetting.cs /r:PresentationFramework.dll /r:PresentationCore.dll /r:WindowsBase.dll

## Usage

Launch GUI:

    XMouseSetting.exe

or simply **double-click** it.

Set to activate a window over mouse:

    XMouseSetting.exe on

Set to activate a window over mouse then to raise after 500 msec:

    XMouseSetting.exe on 500

Off X-Mouse settings (Click to activate and raise):

    XMouseSetting.exe off

NOTICE: To enable new settings, you must **re-login**.

## Other

- Author: bellbind@gmail.com
- Site: [http://github.com/bellbind/XMouseSetting](http://github.com/bellbind/XMouseSetting)
- License: [LGPL](http://www.opensource.org/licenses/lgpl-3.0.html)

## Hints

### Location of `csc.exe` 

On 64bit Windows .NET 3.5:

    c:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe

On 32bit Windows .NET 3.5:

    c:\Windows\Microsoft.NET\Framework\v3.5\csc.exe


