using Microsoft.UI.Xaml.Controls;

namespace WinMediaID
{
    public struct InfoBarMessageQueue
    {
        public string Message { get; set; }

        public InfoBarSeverity Severity { get; set; }

        public bool AutoClose { get; set; }

        public bool IsClosable { get; set; }
    }
}