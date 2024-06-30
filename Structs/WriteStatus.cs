using Microsoft.UI.Dispatching;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace WinMediaID
{
    public struct WriteStatus()
    {
        private static DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private static GlobalProperties globalProperties { get; } = App.Properties;

        private static string _lastStatusMessage;

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
                if (dispatcherQueue is null)
                {
                    Push(consoleMessage);
                }
                else
                {
                    dispatcherQueue.TryEnqueue(() => { Push(consoleMessage); });
                }
            }
            static void Push(string text)
            {
                var _localDateTime = DateTimeOffset.Now.LocalDateTime.ToString("G", DateTimeFormatInfo.InvariantInfo);
                _lastStatusMessage = (text ??= "error");
                var formatMessage = $"\n#[{GlobalProperties.MessageNumber}]. | {{{_localDateTime}}}\n{_lastStatusMessage}\n";
                globalProperties.ConsoleText += formatMessage;
                StatusMessageLog.StringBuilder.Append(formatMessage);
                GlobalProperties.MessageNumber++;
                App.Main_Window.TextBoxScrollViewer.ScrollToVerticalOffset(App.Main_Window.TextBoxScrollViewer.ScrollableHeight);
            }

        }
        public async static Task UpdateConsoleTextAsync(string consoleMessage)
        {
            if (!string.IsNullOrEmpty(consoleMessage))
            {
                if (dispatcherQueue is null)
                {
                    await Push(consoleMessage);
                }
                else
                {
                   dispatcherQueue.TryEnqueue(async()  => { await Push(consoleMessage); });
                }
            }
            static async Task Push(string text)
            {
                var _localDateTime = DateTimeOffset.Now.LocalDateTime.ToString("G", DateTimeFormatInfo.InvariantInfo);
                _lastStatusMessage = (text ??= "error");
                var formatMessage = $"\n#[{GlobalProperties.MessageNumber}]. | {{{_localDateTime}}}\n{_lastStatusMessage}\n";
                await Task.Run(() => { globalProperties.ConsoleText += formatMessage; });
                StatusMessageLog.StringBuilder.Append(formatMessage);
                GlobalProperties.MessageNumber++;
                App.Main_Window.TextBoxScrollViewer.ScrollToVerticalOffset(App.Main_Window.TextBoxScrollViewer.ScrollableHeight);
            }
        }
        public static void UpdateStaticText(string staticStatusMessage)
        {
            if (!string.IsNullOrEmpty(staticStatusMessage))
            {
                if (dispatcherQueue is null)
                {
                    try
                    {
                        globalProperties.StaticStatusMessage = staticStatusMessage;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    dispatcherQueue.TryEnqueue(() => { globalProperties.StaticStatusMessage = staticStatusMessage; });
                }
            }
        }
        public static void UpdateInfoBarText(string infoBarMessasge, bool infoBarAutoClose = true)
        {
            if (!string.IsNullOrEmpty(infoBarMessasge))
            {
                if (dispatcherQueue is null)
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
                    dispatcherQueue.TryEnqueue(() => { App.Main_Window.ShowInfoBar(infoBarMessasge, autoClose: infoBarAutoClose); });
                }
            }
        }

    }
}