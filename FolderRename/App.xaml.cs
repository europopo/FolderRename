using FolderRename.Services;
using Microsoft.UI.Xaml;
using System.Diagnostics;

namespace FolderRename;

public partial class App : Application
{
    public static AppServices Services { get; } = new();
    private Window? _window;

    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            await Services.InitializeAsync();
        }
        catch (Exception exception)
        {
            // A malformed local JSON file or an inaccessible AppData folder must
            // not prevent the user from reaching the application window.
            Debug.WriteLine($"FolderRename initialization failed: {exception}");
        }
        finally
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        Debug.WriteLine($"FolderRename unhandled exception: {args.Exception}");
        args.Handled = true;
        _window?.ShowError(args.Exception.Message);
    }
}
