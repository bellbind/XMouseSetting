// [X-Mouse setting tool on Windows 7/Vista]
// build:
//   csc.exe /t:winexe XMouseSetting.cs /r:PresentationFramework.dll /r:PresentationCore.dll /r:WindowsBase.dll /win32icon:icon.ico
// csc on 64bit .NET 3.5:
//   c:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe
// csc on 32bit .NET 3.5:
//   c:\Windows\Microsoft.NET\Framework\v3.5\csc.exe
namespace Bellbind.XMouseSetting {
  using Microsoft.Win32;
  using System;
  using System.Runtime.InteropServices;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Documents;
  
  public struct Info {
    public bool FollowActivation;
    public bool AutoRaise;
    public int? RaiseTime;
  }
  
  public class XMouseSetting {
    [STAThread]
    static void Main(string[] args) {
      if (args.Length == 0) {
        LaunchUI();
        return;
      }
      
      if (!(args[0] == "on" || args[0] == "off")) {
        ShowHelp();
        Console.WriteLine();
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
          } catch (FormatException ex) {
            Console.WriteLine(ex);
            ShowHelp();
            ShowXMouse();
            return;
          }
        }
      }
      
      ShowXMouse();
      Console.WriteLine();
      PrintNewXMouse(followActivation, autoRaise, raiseTime);
      SetXMouse(followActivation, autoRaise, raiseTime);
      Console.WriteLine();
      Console.WriteLine("To enable new settings, you must re-login.");
    }
    
    static void PrintNewXMouse(bool followActivation, bool autoRaise, int raiseTime) {
      Console.WriteLine("New XMouse Settings:");
      Console.WriteLine("  Follow Activation: {0}", followActivation);
      Console.WriteLine("  Auto Raise: {0}", autoRaise);
      Console.WriteLine("  Raise Time (msec): {0}", raiseTime);
    }
    
    static void ShowHelp() {
      Console.WriteLine("Usage: XMouseSetting [on [NUM]|off|help]");
      Console.WriteLine("Options: ");
      Console.WriteLine(" <nothing>: launch GUI window");
      Console.WriteLine(" on: follow mouse activation");
      Console.WriteLine(" on NUM: follow activation then auto raise after NUM msec");
      Console.WriteLine(" off: disable follow activation/auto raise");
      Console.WriteLine(" help: print help and current settings");
    }
    
    static void ShowXMouse() {
      Info info = GetXMouse();
      Console.WriteLine("Current XMouse Settings:");
      Console.WriteLine("  Follow Activation: {0}", info.FollowActivation);
      Console.WriteLine("  Auto Raise: {0}", info.AutoRaise);
      Console.WriteLine("  Raise Time (msec): {0}", info.RaiseTime);
    }
    
    public static Info GetXMouse() {
      using (var regkey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")) {
        var masks = regkey.GetValue("UserPreferencesMask") as byte[];
        return new Info {
          FollowActivation = (masks[0] & 0x01) != 0,
          AutoRaise = (masks[0] & 0x40) != 0,
          RaiseTime = regkey.GetValue("ActiveWndTrkTimeout") as int?,
        };
      }
    }
    public static void SetXMouse(Info info) {
      SetXMouse(info.FollowActivation, info.AutoRaise, info.RaiseTime ?? 0);
    }
    
    public static void SetXMouse(bool followActivation, bool autoRaise, int raiseTime) {
      using (var regkey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true)) {
        var masks = regkey.GetValue("UserPreferencesMask") as byte[];
        masks[0] = (byte) (followActivation ? masks[0] | 0x01 : masks[0] & ~0x01);
        masks[0] = (byte) (followActivation && autoRaise ? masks[0] | 0x40 : masks[0] & ~0x40);
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
            Content = new TextBlock(new Run("Raise after (msec): ")),
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
        Opacity = 0.9,
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
      var info = GetXMouse();
      followActivation.IsChecked = info.FollowActivation;
      autoRaise.IsChecked = info.AutoRaise;
      raiseTime.Value = info.RaiseTime ?? 0;
      
      application.Run(window);
    }
    
    // from: http://www.eggheadcafe.com/tutorials/aspnet/e5ef4e3e-6f42-4b9b-8834-04366ce32c96/net-lock-logoff-reboot.aspx
    [DllImport("user32.dll")]
    static extern int ExitWindowsEx(int uFlags, int dwReason);
  }

}