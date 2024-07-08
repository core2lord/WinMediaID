using System.Collections.Generic;
using Windows.Storage;

namespace WinMediaID
{
    public partial class GlobalProperties : GlobalPropertiesBase
    {
        public static class UserAppDataSettings
        {
            public static ApplicationDataContainer LocalUserSettings { get; set; } = ApplicationData.Current.LocalSettings;

            public static StorageFolder LocalUserStorageFolder { get; set; } = ApplicationData.Current.LocalFolder;

            private readonly static ApplicationDataCompositeValue UserIgnoreDriveCollection = [];

            public static void AddDriveToIgnoreList(string rootDirectoryLetter, string driveInfoType)
            {
                if (rootDirectoryLetter is not null)
                {
                    UserIgnoreDriveCollection[rootDirectoryLetter] = driveInfoType;
                    LocalUserSettings.Values[nameof(UserIgnoreDriveCollection)] = UserIgnoreDriveCollection;
                    UIStatus.UpdateInfoBarText($"({rootDirectoryLetter}:) drive will be ignored in future scans. ");
                }
            }

            public static void RemoveDriveFromIgnoreList(string rootDirectoryLetter)
            {
                if (rootDirectoryLetter is not null)
                {
                    if (UserIgnoreDriveCollection.ContainsKey(rootDirectoryLetter))
                    {
                        UserIgnoreDriveCollection.Remove(rootDirectoryLetter);
                        UIStatus.UpdateInfoBarText($"({rootDirectoryLetter}:) drive will not be ignored in scans. ");
                    }
                }
            }

#nullable enable

            public static IEnumerable<KeyValuePair<string?, object?>>? GetDrivesOnIgnoredList()
            {
                if (UserIgnoreDriveCollection.Count > 0)
                {
                    foreach (var item in UserIgnoreDriveCollection)
                    {
                        yield return item;
                    }
                }
                yield return new KeyValuePair<string?, object?>(null, null);
            }

#nullable disable
        }
    }
}