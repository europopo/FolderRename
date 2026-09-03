using FolderRename.Services;
using Microsoft.UI.Xaml;

namespace FolderRename;

public partial class App : Application
{
    public static AppServices Services { get; } = new();
    private MainWindow? _window;

    public App()
    {
        CrashLogger.Write("Application process started");

        try
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
        }
        catch (Exception exception)
        {
            CrashLogger.Write("App constructor failed", exception);
            throw;
        }
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        CrashLogger.Write("OnLaunched entered");

        try
        {
            await Services.InitializeAsync();
            CrashLogger.Write("Services initialized");
        }
        catch (Exception exception)
        {
            CrashLogger.Write("Service initialization failed", exception);
        }

        try
        {
            _window = new MainWindow();
            CrashLogger.Write("MainWindow created");
            _window.Activate();
            CrashLogger.Write("MainWindow activated");
        }
        catch (Exception exception)
        {
            CrashLogger.Write("MainWindow startup failed", exception);
            throw;
        }
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
    {
        CrashLogger.Write("WinUI unhandled exception", args.Exception);
        args.Handled = false;
    }
}
