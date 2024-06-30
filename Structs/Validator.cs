using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WinMediaID
{
    public struct Validator
    {
        public string[] ImageFileNames = { "install.esd", "install.wim", "boot.wim" };

        public string[] TestPaths = { $"\\sources\\", $"\\x86\\sources\\", $"\\x64\\sources\\" };

        private StringBuilder _isReadyDrivesArray = new();

        public Validator()
        {
            foreach (var drive in GetIsReadyDrives())
            {
                _isReadyDrivesArray.AppendLine($"{{{drive.RootDirectory.ToString()}}}");
            }
            SystemWatcher.Stop();
        }

        public bool IsValidWindowsInstallationMedia(out string validatedImagePath)
        {
            WriteStatus.UpdateConsoleText($"The following {{{_isReadyDrivesArray.Length / 7}}} storage locations will be checked for valid installation media.\n{_isReadyDrivesArray}");

            foreach (var isReadyDrive in GetIsReadyDrives())
            {
                foreach (var imageFileName in ImageFileNames)
                {
                    foreach (var testPath in TestPaths)
                    {
                        var _pathUnderTest = isReadyDrive + testPath + imageFileName;
                        if (Path.Exists(_pathUnderTest))
                        {
                            validatedImagePath = _pathUnderTest;
                            return true;
                        }
                    }
                }
                WriteStatus.UpdateConsoleText($"No valid installation media found at: {{{isReadyDrive.RootDirectory.ToString()}}}");
            }
            validatedImagePath = null;
            var message = "No valid windows installation media was found(ex. bootable USB storage).\n";
            var message2 = $"If the program does not continue automatically upon inserting new media, you can execute a manual recheck using the button below.";
            WriteStatus.UpdateAllText(message + message2, "Waiting for new media...", "Waiting for new media...", false);
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
}