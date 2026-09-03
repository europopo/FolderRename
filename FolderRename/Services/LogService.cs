using System.Collections.ObjectModel;
using System.Text.Json;
using FolderRename.Models;

namespace FolderRename.Services;

public sealed class LogService
{
    private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FolderRename", "logs.json");
    public ObservableCollection<LogEntry> Entries { get; } = [];

    public async Task InitializeAsync()
    {
        if (!File.Exists(FilePath)) return;
        await using var stream = File.OpenRead(FilePath);
        var entries = await JsonSerializer.DeserializeAsync<List<LogEntry>>(stream) ?? [];
        foreach (var entry in entries.TakeLast(500)) Entries.Add(entry);
    }

    public async Task WriteAsync(LogLevel level, string message)
    {
        Entries.Insert(0, new LogEntry { Level = level, Message = message });
        while (Entries.Count > 500) Entries.RemoveAt(Entries.Count - 1);
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        await using var stream = File.Create(FilePath);
        await JsonSerializer.SerializeAsync(stream, Entries.ToList(), new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task ClearAsync()
    {
        Entries.Clear();
        await WriteFileAsync();
    }

    private async Task WriteFileAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        await using var stream = File.Create(FilePath);
        await JsonSerializer.SerializeAsync(stream, Entries.ToList());
    }
}
