using Microsoft.UI.Dispatching;

namespace WinMediaID.Interfaces
{
    interface IStatusWriter
    {
        void UpdateAllText(string consoleMessage, string staticMessage, string infoBarMessasge, bool infoBarAutoClose = true);

        void UpdateConsoleText(string consoleMessage);

        void UpdateStaticText(string staticStatusMessage);

        void Push(string consoleMessage);
        void UpdateInfoBarText(string infoBarMessasge, bool infoBarAutoClose = true);
    }
}