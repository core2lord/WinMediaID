using Microsoft.UI.Dispatching;
using System;
using System.Globalization;

namespace WinMediaID
{
    public struct UIStatus()
    {
        private static readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private static readonly GlobalProperties _globalProperties = App.Properties;

        private static string _lastStatusMessage;

        public struct ProgressRing()
        {
            public static void Start()
            {
                if (!_globalProperties.IsPRingActive)
                {
                    if (!_dispatcherQueue.TryEnqueue(() =>
                    {
                        _globalProperties.IsPRingActive = true;
                    }))
                    {
                        UpdateConsoleText("Error 0x011");
                    }
                }
            }

            public static void Stop()
            {
                UpdateConsoleText("Ring to stop\nStatus=" + _globalProperties.IsPRingActive.ToString());
                if (_globalProperties.IsPRingActive)
                {
                    if (!_dispatcherQueue.TryEnqueue(() =>
                    {
                        _globalProperties.IsPRingActive = false;
                    }))
                    {
                        UpdateConsoleText("Error 0x011");
                    }
                }
            }
        }

        public static void UpdateAllText(string consoleMessage, string staticMessage, string infoBarMessasge, bool infoBarAutoClose = true)
        {
            if (!string.IsNullOrEmpty(consoleMessage))
            {
                UpdateConsoleText(consoleMessage);
            }
            if (!string.IsNullOrEmpty(infoBarMessasge))
            {
                UpdateInfoBarText(infoBarMessasge, infoBarAutoClose);
            }
            if (!string.IsNullOrEmpty(staticMessage))
            {
                UpdateStaticText(staticMessage);
            }
        }

        public static void UpdateConsoleText(string consoleMessage)
        {
            if (!string.IsNullOrEmpty(consoleMessage))
            {
                if (_dispatcherQueue is null)
                {
                    Push(consoleMessage);
                }
                else
                {
                    _dispatcherQueue.TryEnqueue(() => { Push(consoleMessage); });
                }
            }
            static void Push(string text)
            {
                var _localDateTime = DateTimeOffset.Now.LocalDateTime.ToString("G", DateTimeFormatInfo.InvariantInfo);
                _lastStatusMessage = (text ??= "error");
                var formatMessage = $"\n#[{GlobalProperties.MessageNumber}]. | {{{_localDateTime}}}\n{_lastStatusMessage}\n";
                _globalProperties.ConsoleText += formatMessage;
                StatusMessageLog.StringBuilder.Append(formatMessage);
                GlobalProperties.MessageNumber++;
                App.Main_Window.TextBoxScrollViewer.ScrollToVerticalOffset(App.Main_Window.TextBoxScrollViewer.ScrollableHeight);
            }
        }

        //public async static Task UpdateConsoleTextAsync(string consoleMessage)
        //{
        //    if (!string.IsNullOrEmpty(consoleMessage))
        //    {
        //        if (_dispatcherQueue is null)
        //        {
        //            await Push(consoleMessage);
        //        }
        //        else
        //        {
        //            _dispatcherQueue.TryEnqueue(async () => { await Push(consoleMessage); });
        //        }
        //    }
        //    static async Task Push(string text)
        //    {
        //        var _localDateTime = DateTimeOffset.Now.LocalDateTime.ToString("G", DateTimeFormatInfo.InvariantInfo);
        //        _lastStatusMessage = (text ??= "error");
        //        var formatMessage = $"\n#[{GlobalProperties.MessageNumber}]. | {{{_localDateTime}}}\n{_lastStatusMessage}\n";
        //        await Task.Run(() => { _globalProperties.ConsoleText += formatMessage; });
        //        StatusMessageLog.StringBuilder.Append(formatMessage);
        //        GlobalProperties.MessageNumber++;
        //        App.Main_Window.TextBoxScrollViewer.ScrollToVerticalOffset(App.Main_Window.TextBoxScrollViewer.ScrollableHeight + 10);
        //    }
        //}

        public static void UpdateStaticText(string staticStatusMessage)
        {
            if (!string.IsNullOrEmpty(staticStatusMessage))
            {
                if (_dispatcherQueue is null)
                {
                    try
                    {
                        _globalProperties.StaticStatusMessage = staticStatusMessage;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    _dispatcherQueue.TryEnqueue(() => { _globalProperties.StaticStatusMessage = staticStatusMessage; });
                }
            }
        }

        public static void UpdateInfoBarText(string infoBarMessasge, bool infoBarAutoClose = true)
        {
            if (!string.IsNullOrEmpty(infoBarMessasge))
            {
                if (_dispatcherQueue is null)
                {
                    try
                    {
                        App.Main_Window.ShowInfoBar(infoBarMessasge, autoClose: infoBarAutoClose);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    _dispatcherQueue.TryEnqueue(() => { App.Main_Window.ShowInfoBar(infoBarMessasge, autoClose: infoBarAutoClose); });
                }
            }
        }
    }
}