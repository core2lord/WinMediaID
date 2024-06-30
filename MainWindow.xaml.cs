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

    public int WindowWidth { get; set; } = 480;

    internal SizeInt32 DisplayPixelsXY { get; private set; }

    public static List<InfoBarMessageQueue> InfoBarMessageQueues { get; private set; }


    public MainWindow()
    {
        DisplayPixelsXY = new SizeInt32(WindowWidth, WindowHeight);
        InitializeComponent();
        SetMainWindowParamenters();
        AppStartup();
    }

    private void AppStartup()
    {
        ShowInfoBar("Searching for Windows installation media...");
        GlobalProperties.StaticStatusMessage = "Searching for media";
        MediaValidationCheck();
    }

    public void MediaValidationCheck()
    { 
        Validator validator = new();
        if (validator.IsValidWindowsInstallationMedia(out string validatedPath))
        {
            if (validatedPath is not null)
            {
                MediaIdCommand.Run(validatedPath);
                SystemWatcher.Stop();
            }
            else
            {
                WriteStatus.UpdateConsoleText("Error during path validation occured.");
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
        OverlappedPresenter presenter = AppWindow.Presenter as OverlappedPresenter;
        presenter.IsResizable = false;
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
        if (GlobalProperties.IsInfoBarOpen)
        {
            GlobalProperties.IsInfoBarOpen = false;
        }
    } 

    private void ButtonRescan_Click(object sender, RoutedEventArgs e)
    {
        MediaValidationCheck();
    }
}