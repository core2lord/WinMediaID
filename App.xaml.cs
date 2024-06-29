using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure, and more about our
// project templates, see: http://aka.ms/winui-project-info.

namespace WinMediaID;

/// <summary>
///    Provides application-specific behavior to supplement the default
///    Application class.
/// </summary>
public partial class App : Application
{
    #region Fields

    #endregion Fields

    #region Public Constructors

    /// <summary>
    ///    Initializes the singleton application object. This is the first line
    ///    of authored code executed, and as such is the logical equivalent of
    ///    main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        Properties = new GlobalProperties();
    }

    #endregion Public Constructors

    #region Properties

    public static MainWindow Main_Window { get; set; }

    public static GlobalProperties Properties { get; private set; }

    #endregion Properties

    #region Protected Methods

    /// <summary>
    ///    Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Main_Window = new MainWindow();
        Main_Window.Activate();
    }

    #endregion Protected Methods
}