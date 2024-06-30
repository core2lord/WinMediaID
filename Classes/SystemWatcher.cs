using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace WinMediaID
{
    public class SystemWatcher
    {
        #region Fields

        private static readonly string _logFileName = $"\\win_media_id_log-{DateTimeOffset.Now.ToLocalTime().ToString("MM%dd%yyyy'T'HH%mm%ss")}.txt";

        private static readonly string _userDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.None);

        private static GlobalProperties _globalProperties = App.Properties;

        private static bool _isWatcherEnumerationComplete = false;

        private static bool _isWatcherCanStart = true;

        private static DeviceWatcher _devWatcher;

        private static bool _isWatcherStopRequested = false;

        #endregion Fields

        #region Private Methods

        private static async Task AppendLogFileAsync()
        {
            if (Directory.Exists(_userDesktopPath))
            {
               await File.AppendAllTextAsync(_userDesktopPath + _logFileName, StatusMessageLog.StringBuilder.ToString());
               WriteStatus.UpdateConsoleText($"Created/Updated log file.\n{{{_userDesktopPath}}}");
               StatusMessageLog.StringBuilder.Clear();
            }
        }

        private static void DeviceWatcher_AddedOrUpdated(DeviceWatcher sender, DeviceInformation args)
        {
            if (_isWatcherEnumerationComplete)
            {
                var message = $"::Device Added::\n{args.Name}\n{args.Id}\nRescanning...";
                WriteStatus.UpdateAllText(message, "Rescanning...", "Rescanning...");
                App.Main_Window.MediaValidationCheck();
            }
        }

        private static void DeviceWatcher_AddedOrUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (_isWatcherEnumerationComplete)
            {
                var message = $"::Device Updated::\n{args.Id}\nRescanning...";
                WriteStatus.UpdateAllText(message, "Rescanning...", "Rescanning...", true);
                App.Main_Window.MediaValidationCheck();
            }
        }

        private static void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            _isWatcherEnumerationComplete = true;
            Task.Run(() => { AppendLogFileAsync().GetAwaiter().GetResult(); });
        }

        private static void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var message = $"A device was removed.";
            WriteStatus.UpdateConsoleText(message);
        }

        #endregion Private Methods

        #region Public Methods

        private static void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            _isWatcherCanStart = true;
            _isWatcherEnumerationComplete = false;
        }

        private static bool GetCanStart()
        {
            if (_devWatcher is not null)
            {
                if (!_isWatcherCanStart && _isWatcherStopRequested)
                {
                    int deviceWatcherStartAttempts = 0;
                    int messageCounter = 0;
                    int timeOutPeriod = 30;
                    int taskDelay = 1000;
                    while (!_isWatcherCanStart)
                    {
                        timeOutPeriod--;
                        Task.Delay(taskDelay);
                        deviceWatcherStartAttempts++;
                        messageCounter++;
                        if (messageCounter > 5)
                        {
                            Task.Run(() =>
                            {
                                WriteStatus.UpdateConsoleText($"Waiting to start DeviceWatcher service: Attempt#{deviceWatcherStartAttempts}.\nTimeout in {timeOutPeriod} seconds.");
                            });
                            messageCounter = 0;
                        }
                        if (timeOutPeriod <= 0)
                        {
                            WriteStatus.UpdateConsoleText("DeviceWatcher service failed to start within timeout period.");
                            return false;
                        }
                    }
                    WriteStatus.UpdateConsoleText($"DeviceWatcher service started after::({deviceWatcherStartAttempts}) seconds");
                    return true;
                }
                else if (_isWatcherCanStart && _isWatcherStopRequested)
                {
                    if (_devWatcher.Status != (DeviceWatcherStatus.Created | DeviceWatcherStatus.Started))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }

        public static void TryStart()
        {
            if (GetCanStart())
            {
                _devWatcher = DeviceInformation.CreateWatcher();
                _devWatcher.Updated += DeviceWatcher_AddedOrUpdated;
                _devWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
                _devWatcher.Removed += DeviceWatcher_Removed;
                _devWatcher.Added += DeviceWatcher_AddedOrUpdated;
                _devWatcher.Stopped += DeviceWatcher_Stopped;
                _devWatcher.Start();
                _isWatcherCanStart = false;
                _isWatcherStopRequested = false;
            }
        }

        public static void Stop()
        {
            if (_devWatcher is not null)
            {
                _isWatcherStopRequested = true;
                if (_devWatcher.Status != (DeviceWatcherStatus.Stopping | DeviceWatcherStatus.Stopped | DeviceWatcherStatus.Aborted))
                {
                    _devWatcher.Stop();
                    _isWatcherEnumerationComplete = false;
                }
                else
                {
                    while (_devWatcher.Status == (DeviceWatcherStatus.Stopping | DeviceWatcherStatus.Stopped | DeviceWatcherStatus.Aborted))
                    {
                        Task.Delay(1000);
                        WriteStatus.UpdateConsoleText("Attempting to stop the DeviceWatcher service.");
                    }
                    _devWatcher.Stop();
                }
            }
        }

        #endregion Public Methods
    }
}