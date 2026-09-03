using System.Text.Json;
using FolderRename.Models;

namespace FolderRename.Services;

public sealed class SettingsStore
{
    private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FolderRename", "settings.json");
    private readonly SemaphoreSlim _gate = new(1, 1);
    public ScheduleSettings Current { get; private set; } = new();

    public async Task LoadAsync()
    {
        if (!File.Exists(FilePath)) return;
        await using var stream = File.OpenRead(FilePath);
        Current = await JsonSerializer.DeserializeAsync<ScheduleSettings>(stream) ?? new();
    }

    public async Task SaveAsync(ScheduleSettings settings)
    {
        await _gate.WaitAsync();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            Current = settings;
            await using var stream = File.Create(FilePath);
            await JsonSerializer.SerializeAsync(stream, settings, new JsonSerializerOptions { WriteIndented = true });
        }
        finally { _gate.Release(); }
    }
}
