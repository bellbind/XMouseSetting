// [X-Mouse setting tool on Windows 7/Vista]
// build:
//   csc.exe /t:winexe XMouseSetting.cs /r:PresentationFramework.dll /r:PresentationCore.dll /r:WindowsBase.dll /win32icon:icon.ico
// csc on 64bit .NET 3.5:
//   c:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe
// csc on 32bit .NET 3.5:
//   c:\Windows\Microsoft.NET\Framework\v3.5\csc.exe 
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

class XMouseSetting {
  [System.STAThread]
  static void Main(string[] args) {
    if (args.Length == 0) {
      LaunchUI();
      return;
    }
    
    if (!(args[0] == "on" || args[0] == "off")) {
      ShowHelp();
      System.Console.WriteLine();
      ShowXMouse();
      return;
    }
    
    var followActivation = false;
    var autoRaise = false;
    var raiseTime = 0;
    
    if (args[0] == "on") {
      followActivation = true;
      if (args.Length == 2) {
        try {
          raiseTime = int.Parse(args[1]);
          autoRaise = true;
        } catch (System.FormatException ex) {
          System.Console.WriteLine(ex);
          ShowHelp();
          ShowXMouse();
          return;
        }
      }
    }
    
    ShowXMouse();
    System.Console.WriteLine();
    PrintNewXMouse(followActivation, autoRaise, raiseTime);
    SetXMouse(followActivation, autoRaise, raiseTime);
    System.Console.WriteLine();
    System.Console.WriteLine("To enable new settings, you must re-login.");
  }
  
  static void PrintNewXMouse(bool followActivation, bool autoRaise, int raiseTime) {
    System.Console.WriteLine("New XMouse Settings:");
    System.Console.WriteLine("  Follow Activation: {0}", followActivation);
    System.Console.WriteLine("  Auto Raise: {0}", autoRaise);
    System.Console.WriteLine("  Raise Time (msec): {0}", raiseTime);
  }
  
  static void ShowHelp() {
    System.Console.WriteLine("Usage: XMouseSetting [on [NUM]|off|help]");
    System.Console.WriteLine("Options: ");
    System.Console.WriteLine(" <nothing>: launch GUI");
    System.Console.WriteLine(" on: follow mouse activation");
    System.Console.WriteLine(" on NUM: follow activation then auto raise after NUM msec");
    System.Console.WriteLine(" off: disable follow activation/auto raise");
  }
  
  static void ShowXMouse() {
    using (var regkey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")) {
      var masks = regkey.GetValue("UserPreferencesMask") as byte[];
      System.Console.WriteLine("Current XMouse Settings:");
      System.Console.WriteLine("  Follow Activation: {0}", (masks[0] & 0x01) != 0);
      System.Console.WriteLine("  Auto Raise: {0}", (masks[0] & 0x40) != 0);
      System.Console.WriteLine("  Raise Time (msec): {0}", regkey.GetValue("ActiveWndTrkTimeout"));
    }
  }
  
  static void SetXMouse(bool followActivation, bool autoRaise, int raiseTime) {
    using (var regkey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true)) {
      var masks = regkey.GetValue("UserPreferencesMask") as byte[];
      if (followActivation) {
        masks[0] |= 0x01;
      } else {
        masks[0] = (byte) (masks[0] & ~0x01);
      }
      if (autoRaise) {
        masks[0] |= 0x40;
      } else {
        masks[0] = (byte) (masks[0] & ~0x40);
      }
      regkey.SetValue("UserPreferencesMask", masks);
      if (autoRaise) {
        regkey.SetValue("ActiveWndTrkTimeout", raiseTime);
      } else {
        regkey.DeleteValue("ActiveWndTrkTimeout", false);
      }
    }
  }
  
  // use WPF
  static void LaunchUI() {
    // widgets
    var followActivation = new CheckBox {
      Content = new TextBlock(new Run("Activate a window when mouse entered")),
      IsChecked = true,
      IsEnabled = true,
    };
    var autoRaise = new CheckBox {
      Content = new TextBlock(new Run("Auto raise")),
      IsChecked = true,
      IsEnabled = true,
    };
    var raiseTime = new Slider {
      Value = 500,
      Minimum = 0,
      Maximum = 1000,
      AutoToolTipPlacement = AutoToolTipPlacement.BottomRight,
      IsMoveToPointEnabled = true,
      TickFrequency = 10,
      TickPlacement = TickPlacement.BottomRight,
      IsSnapToTickEnabled = true,
      IsEnabled = true,
    };
    var panelRaiseTime = new DockPanel {
      Children = {
        new Label {
          Content = new TextBlock(new Run("Raise time: ")),
        },
        raiseTime,
      },
    };
    var save = new Button {
      Content = new TextBlock(new Run("save")),
      Width = 150,
    };
    var window = new Window {
      Width = 300,
      Height = 150,
      //Opacity = 0.9,
      Title = "X-Mouse Settings",
      Content = new StackPanel {
        Children = {
          new GroupBox {
            Header = "X-Mouse Settings", 
            Content = new StackPanel {
              Children = {
                followActivation,
                autoRaise,
                panelRaiseTime,
              },
            },
          },
          save,
        },
      },
    };
    var application = new Application();
    
    // events
    followActivation.Checked += (sender, ev) => {
      autoRaise.IsEnabled = true;
    };
    followActivation.Unchecked += (sender, ev) => {
      autoRaise.IsEnabled = false;
      autoRaise.IsChecked = false;
    };
    autoRaise.Checked += (sender, ev) => {
      panelRaiseTime.IsEnabled = true;
    };
    autoRaise.Unchecked += (sender, ev) => {
      panelRaiseTime.IsEnabled = false;
    };
    save.Click += (sender, ev) => {
      SetXMouse(followActivation.IsChecked ?? false,
                autoRaise.IsChecked ?? false,
                (int) raiseTime.Value);
      var result = MessageBox.Show(
        "To enable new settings, you must re-login.\n\nLog-off now?",
        "XMouse Settings Updated",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question);
      application.Shutdown();
      if (result == MessageBoxResult.Yes) {
        ExitWindowsEx(0, 0); // Log-off
      }
    };
    
    // initialize
    using (var regkey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")) {
      var masks = regkey.GetValue("UserPreferencesMask") as byte[];
      followActivation.IsChecked = (masks[0] & 0x01) != 0;
      autoRaise.IsChecked = (masks[0] & 0x40) != 0;
      raiseTime.Value = (int) (regkey.GetValue("ActiveWndTrkTimeout") ?? 0);
    }
    
    application.Run(window);
  }
  
  // from: http://www.eggheadcafe.com/tutorials/aspnet/e5ef4e3e-6f42-4b9b-8834-04366ce32c96/net-lock-logoff-reboot.aspx
  [DllImport("user32.dll")]
  static extern int ExitWindowsEx(int uFlags, int dwReason);
}
