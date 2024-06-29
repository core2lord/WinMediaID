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