using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace WinMediaID;

public partial class GlobalProperties : GlobalPropertiesBase
{
    private string commandText;

    private bool isInfoBarOpen;

    private string infoBarMessage;

    private InfoBarSeverity infoBarSeverity;

    private bool infoBarIsClosable;

    private string staticStatusMessage;

    private bool isPRingActive;

    public bool IsPRingActive { get => isPRingActive; set => SetProperty(ref isPRingActive, value, nameof(IsPRingActive)); }

    public static int MessageNumber { get; set; }

    public string StaticStatusMessage { get => staticStatusMessage; set => SetProperty(ref staticStatusMessage, value, nameof(StaticStatusMessage)); }

    public bool InfoBarIsClosable { get => infoBarIsClosable; set => SetProperty(ref infoBarIsClosable, value, nameof(InfoBarIsClosable)); }

    public InfoBarSeverity InfoBarSeverity { get => infoBarSeverity; set => SetProperty(ref infoBarSeverity, value, nameof(InfoBarSeverity)); }

    public string InfoBarMessage { get => infoBarMessage; set => SetProperty(ref infoBarMessage, value, nameof(InfoBarMessage)); }

    public bool IsInfoBarOpen { get => isInfoBarOpen; set => SetProperty(ref isInfoBarOpen, value, nameof(IsInfoBarOpen)); }

    public string ConsoleText { get => commandText; set => SetProperty(ref commandText, value, nameof(ConsoleText)); }

    public ObservableCollection<VisualIsReadyDrive> VisualIsReadyDriveCollection { get; set; } = [];
}