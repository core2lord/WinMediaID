using Microsoft.UI.Dispatching;
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

        private static readonly DirectoryInfo DInfoLogPath = new(userDesktopPath);

        private static readonly string logFileName = $"\\win_media_id_log-{DateTimeOffset.Now.ToLocalTime().ToString("MM%dd%yyyy'T'HH%mm%ss")}.txt";

        private static readonly string userDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.None);

       // private static DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

      //  private static GlobalProperties globalProperties = App.Properties;

        private static bool isWatcherEnumerationComplete = false;

        private static bool isWatcherCanStart = true;

        private static DeviceWatcher WatcherObject;

        #endregion Fields

        #region Private Methods

    /*    private static async Task AppendLogFileAsync()
        {
            if (Directory.Exists(userDesktopPath))
            {
                DirectorySecurity directorySecurity = new(userDesktopPath, AccessControlSections.Access | AccessControlSections.Audit);
                var userAccount = new NTAccount(Environment.UserName);
                var accessRules = directorySecurity.GetAccessRules(true, true, typeof(NTAccount));

                foreach (FileSystemAccessRule permission in accessRules)
                {
                    WriteStatus.UpdateConsoleText($"Checking {userDesktopPath} permissions for {userAccount.ToString()}");
                    if (permission.AccessControlType == AccessControlType.Allow)
                    {
                        await File.AppendAllTextAsync(userDesktopPath + logFileName, StatusMessageLog.StringBuilder.ToString());
                        WriteStatus.UpdateConsoleText($"Appened log to file\n{{{userDesktopPath}}}");
                    }
                    WriteStatus.UpdateConsoleText($"Error writing log to file\n{{{userDesktopPath}}}\nPermissions Error Occured.{permission.AccessControlType.ToString()}");
                }
                StatusMessageLog.StringBuilder.Clear();
            }
            else
            {
                WriteStatus.UpdateConsoleText($"Error creating logfile\n{{{userDesktopPath}}}");
            }
        }
    */
        private static void DeviceWatcher_AddedOrUpdated(DeviceWatcher sender, DeviceInformation args)
        {
            if (isWatcherEnumerationComplete)
            {
                var message = "::Device Added::\n" + args.Name + Environment.NewLine + args.Id + Environment.NewLine + args.Kind + Environment.NewLine + "Rescanning...";
                WriteStatus.UpdateAllText(message, "Rescanning...", "Rescanning...");
                MainWindow.MediaValidationCheck();
            }
        }

        private static void DeviceWatcher_AddedOrUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (isWatcherEnumerationComplete)
            {
                var message = "Device updated\n" + args.Id + Environment.NewLine + args.Kind + Environment.NewLine + "Rescanning...";
                WriteStatus.UpdateAllText(message, "Rescanning...", "Rescanning...", true);
                MainWindow.MediaValidationCheck();
            }
        }

        private static void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            var message = "No valid windows installation media was found(ex. bootable USB storage).";
            var message2 = $"If the program does not continue automatically upon inserting new media, click the recheck button below.";
            WriteStatus.UpdateAllText(message, "Waiting for new media...", message2, false);
            isWatcherEnumerationComplete = true;
           // Task.Run(() => { AppendLogFileAsync().GetAwaiter(); });
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
            isWatcherCanStart = true;
        }

        //private static async Task WaitStartDeviceWatcher()
        //{
        //    while (!isWatcherCanStart)
        //    {
        //        await Task.Delay(750);
        //    }
        //    WatcherObject.Start();
        //}

        public static void Start()
        {
            WatcherObject = DeviceInformation.CreateWatcher();
            WatcherObject.Updated += DeviceWatcher_AddedOrUpdated;
            WatcherObject.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            WatcherObject.Removed += DeviceWatcher_Removed;
            WatcherObject.Added += DeviceWatcher_AddedOrUpdated;
            WatcherObject.Stopped += DeviceWatcher_Stopped;
            int deviceWatcherStartAttempts = 0;
            int taskDelay = 750;
            while (!isWatcherCanStart)
            {
                Task.Delay(taskDelay);
                WriteStatus.UpdateConsoleText("Waiting to start DeviceWatcher service: Try#" + deviceWatcherStartAttempts++);
            }
            WatcherObject.Start();
            isWatcherCanStart = false;
            double timeTotalToStartService = (deviceWatcherStartAttempts * taskDelay / 1000);
            WriteStatus.UpdateConsoleText("DeviceWatcher service success after::(" + timeTotalToStartService + ") seconds");

        }

        public static void Stop()
        {
            if (WatcherObject.Status != (DeviceWatcherStatus.Stopping | DeviceWatcherStatus.Stopped | DeviceWatcherStatus.Aborted))
            {
                WatcherObject.Stop();
                isWatcherEnumerationComplete = false;
       //       Task.Run(() => { AppendLogFileAsync().GetAwaiter(); });
            }
            else
            {
                while (WatcherObject.Status == (DeviceWatcherStatus.Stopping | DeviceWatcherStatus.Stopped | DeviceWatcherStatus.Aborted))
                {
                    Task.Delay(1000);
                    WriteStatus.UpdateConsoleText("Attempting to stop the DeviceWatcher service.");
                }
                WatcherObject.Stop();
            }
        }

        #endregion Public Methods
    }
}