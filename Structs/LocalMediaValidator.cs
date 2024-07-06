using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.UI;

namespace WinMediaID
{
    internal readonly struct LocalMediaValidator
    {
        private readonly GlobalProperties _globalProperties = App.Properties;

        private readonly string[] ImageFileNames = { "install.esd", "install.wim", "boot.wim" };

        private readonly string[] RelativeSourcePath = { $"\\sources\\", $"\\x86\\sources\\", $"\\x64\\sources\\" };

        private readonly List<DriveInfo> _isReadyDriveInfoCollection = [];

        private readonly StringBuilder _isReadyRootDirectoryAsStringCollection = new();

        public LocalMediaValidator()
        {
            foreach (var drive in GetIsReadyDrives())
            {
                var rootDirectoryString = drive.RootDirectory.ToString();
                _isReadyDriveInfoCollection.Add(drive);
                _isReadyRootDirectoryAsStringCollection.AppendLine($"{{{rootDirectoryString}}}");
                var addToUI = new VisualIsReadyDrive(rootDirectoryString, drive.DriveType );
                _globalProperties.VisualIsReadyDriveCollection.Add(addToUI);
            }
        }

        public readonly bool IsValidWindowsInstallationMedia(out string validatedImagePath)
        {
            if (_isReadyDriveInfoCollection.Count <= 1)
            {
                UIStatus.UpdateConsoleText($"[{_isReadyDriveInfoCollection.Count}] storage device on local system; [{{{Environment.MachineName,15}}}] was found.\nPlease make sure the source in question is connected, is accessable and showing up as an addtional storage device. \n{_isReadyRootDirectoryAsStringCollection}");
            }
            else
            {
                UIStatus.UpdateConsoleText($"Found [{_isReadyDriveInfoCollection.Count}] storage devices on local system; [{{{Environment.MachineName,15}}}] locations will be checked for valid installation media.\n{_isReadyRootDirectoryAsStringCollection}");
            }
            UIStatus.ProgressRing.Start();
            foreach (var isReadyDriveUnderTest in GetIsReadyDrives())
            {
                foreach (var imageFileName in ImageFileNames)
                {
                    foreach (var testPath in RelativeSourcePath)
                    {
                        var _fullyQualifiedPathUnderTest = isReadyDriveUnderTest + testPath + imageFileName;
                        if (Path.Exists(_fullyQualifiedPathUnderTest))
                        {
                            validatedImagePath = _fullyQualifiedPathUnderTest;
                            UIStatus.UpdateConsoleText($"Windows Installation Media found!\n [{isReadyDriveUnderTest.RootDirectory}]");
                            return true;
                        }
                    }
                }
                UIStatus.UpdateConsoleText($"{{{isReadyDriveUnderTest.DriveType} - {isReadyDriveUnderTest.RootDirectory}}} doesn't appear to be a windows installation media source.");
            }
            validatedImagePath = null;
            var message = "A valid windows installation media source was not found.\n";
            var message2 = $"App should continue automatically upon inserting a new media source, however a manual recheck can be called using the button below.";
            UIStatus.UpdateAllText(message + message2, "Waiting for new media...", "Waiting for new media...", false);
            UIStatus.ProgressRing.Stop();
            return false;
        }

        private static IEnumerable<DriveInfo> GetIsReadyDrives()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    yield return drive;
                }
            }
        }
    }

    public class VisualIsReadyDrive
    {
        private DriveType driveInfoType;

        public VisualIsReadyDrive(string rootDirectory, DriveType driveInfoType)
        {
            RootDirectory = rootDirectory;
            DriveInfoType = driveInfoType;
        }

        public string RootDirectory { get; set; }
        public SolidColorBrush BackgroundStatus { get; set; } = new SolidColorBrush(new Color() { R=189, G=69, B=79}  );
        public string ConvertedDriveTypeString { get; private set; }

        public DriveType DriveInfoType
        {
            get
            {
                return driveInfoType;
            }
            set
            {
                switch (value)
                {
                    case DriveType.Unknown:
                        { ConvertedDriveTypeString = "Other\\Unknown Drive"; }
                        break;
                    case DriveType.NoRootDirectory:
                        { ConvertedDriveTypeString = "Invalid Drive"; }
                        break;
                    case DriveType.Removable:
                        { ConvertedDriveTypeString = "USB\\Flash Drive"; }
                        break;
                    case DriveType.Fixed:
                        { ConvertedDriveTypeString = "Local Disk Drive"; }
                        break;
                    case DriveType.Network:
                        { ConvertedDriveTypeString = "Network Drive"; }
                        break;
                    case DriveType.CDRom:
                        { ConvertedDriveTypeString = "CD\\DVD\\BD Drive"; }
                        break;
                    case DriveType.Ram:
                        { ConvertedDriveTypeString = "RAM Drive"; }
                        break;
                    default:
                        break;
                }
                driveInfoType = value;
            }
        }
    }
}