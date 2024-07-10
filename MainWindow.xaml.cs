using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Graphics;
using Windows.Win32.Foundation;
using static Windows.Win32.PInvoke;

namespace WinMediaID;

public sealed partial class MainWindow : Window
{
    public GlobalProperties GlobalProperties { get; } = App.Properties;

    public int WindowHeight { get; set; } = 660;

    public int WindowWidth { get; set; } = 780;

    internal SizeInt32 DisplayPixelsXY { get; private set; }

    OverlappedPresenter OverlappedPresenter { get; set; }

    public static List<InfoBarMessageQueue> InfoBarMessageQueues { get; private set; }

    public MainWindow()
    {
        DisplayPixelsXY = new SizeInt32(WindowWidth, WindowHeight);
        InitializeComponent();
        SetMainWindowParamenters();
        AppStartup();
        ApplyUserDriveIgnoreList();
    }

    private void ApplyUserDriveIgnoreList()
    {
        var ignoreList = GlobalProperties.UserAppDataSettings.GetDrivesOnIgnoredList();
        if (ignoreList is not null)
        {
            foreach (var item in ignoreList)
            {
                if (item.Key != null && item.Value != null)
                {
                    GlobalProperties.UserAppDataSettings.AddDriveToIgnoreList(item.Key, item.Value.ToString());
                }
            }
        }
    }

    private static void AppStartup()
    {
        UIStatus.UpdateAllText("Searching for Windows installation media...", "Searching for media", "Searching...");
        MediaValidationCheck();
    }

    public static void MediaValidationCheck()
    {
        LocalMediaValidator validator = new();
        if (validator.IsValidWindowsInstallationMedia(out string validatedPath))
        {
            if (validatedPath is not null)
            {
                SystemWatcher.Stop();
                MediaIdCommand.Run(validatedPath);
                return;
            }
            else
            {
                UIStatus.UpdateConsoleText("Error during path validation occured.");
            }
        }
        SystemWatcher.TryStart();
    }

    private void SetMainWindowParamenters()
    {
        var hWnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(this);
        SetWindowPos(hWnd, default, 0, 0, WindowWidth, WindowHeight,
                     Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOMOVE | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
        AppWindow.ResizeClient(new SizeInt32(WindowWidth, WindowHeight));
        OverlappedPresenter = AppWindow.Presenter as OverlappedPresenter;
        OverlappedPresenter.IsResizable = false;
    }

    public void ShowInfoBar(string message, InfoBarSeverity severity = InfoBarSeverity.Informational, bool autoClose = true, bool isClosable = false)
    {
        GlobalProperties.InfoBarMessage = message;
        GlobalProperties.InfoBarSeverity = severity;
        GlobalProperties.InfoBarIsClosable = isClosable;
        GlobalProperties.IsInfoBarOpen = true;
        if (autoClose)
        {
            var timer = DispatcherQueue.CreateTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 2500);
            timer.IsRepeating = false;
            timer.Tick += TimerElasped_HideInfoBar;
            timer.Start();
        }
    }

    public void HideInfoBar()
    {
        if (GlobalProperties.IsInfoBarOpen)
        {
            GlobalProperties.IsInfoBarOpen = false;
        }
    }

    private void TimerElasped_HideInfoBar(DispatcherQueueTimer sender, object args)
    {
        HideInfoBar();
    }

    private void ButtonRescan_Click(object sender, RoutedEventArgs e)
    {
        UIStatus.UpdateConsoleText($"clientSize={AppWindow.ClientSize.Width}x{AppWindow.ClientSize.Height}\nsize={AppWindow.Size.Width}x{AppWindow.Size.Height}");

        //AppWindow.ResizeClient(WindowWidth, AppWindow.)
    }

    private void DriveListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var addIgnore = e.AddedItems;
        var removeIgnore = e.RemovedItems;

        if (addIgnore.Count > 0)
        {
            for (int i = 0; i < addIgnore.Count; i++)
            {
                var item = addIgnore[i] as VisualIsReadyDrive;
                GlobalProperties.UserAppDataSettings.AddDriveToIgnoreList(item.RootDirectoryLetter.ToString(), item.DriveInfoType.ToString());
            }
        }
        if (removeIgnore.Count > 0)
        {
            for (int i = 0; i < removeIgnore.Count; i++)
            {
                var item = removeIgnore[i] as VisualIsReadyDrive;
                GlobalProperties.UserAppDataSettings.RemoveDriveFromIgnoreList(item.RootDirectoryLetter.ToString());
            }
        }
    }
}