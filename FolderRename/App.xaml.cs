using FolderRename.Services;
using Microsoft.UI.Xaml;

namespace FolderRename;

public partial class App : Application
{
    public static AppServices Services { get; } = new();
    private Window? _window;

    public App() => InitializeComponent();

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await Services.InitializeAsync();
        _window = new MainWindow();
        _window.Activate();
    }
}
