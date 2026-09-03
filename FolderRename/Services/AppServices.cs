using FolderRename.Models;

namespace FolderRename.Services;

/// <summary>Application composition root. New services are registered here to keep pages decoupled.</summary>
public sealed class AppServices
{
    public SettingsStore Settings { get; } = new();
    public LogService Logs { get; } = new();
    public RenameService Renamer { get; } = new();
    public SchedulerService Scheduler { get; }

    public AppServices() => Scheduler = new SchedulerService(Settings, Logs, Renamer);

    public async Task InitializeAsync()
    {
        await Settings.LoadAsync();
        await Logs.InitializeAsync();
        Scheduler.Start();
    }
}
