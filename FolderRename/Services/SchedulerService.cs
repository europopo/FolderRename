using FolderRename.Models;

namespace FolderRename.Services;

public sealed class SchedulerService
{
    private readonly SettingsStore _settings;
    private readonly LogService _logs;
    private readonly RenameService _renamer;
    private CancellationTokenSource? _cancellation;
    private int _running;

    public SchedulerService(SettingsStore settings, LogService logs, RenameService renamer) => (_settings, _logs, _renamer) = (settings, logs, renamer);

    public void Start()
    {
        _cancellation ??= new CancellationTokenSource();
        _ = MonitorAsync(_cancellation.Token);
    }

    private async Task MonitorAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync(token))
        {
            var settings = _settings.Current;
            if (settings.IsEnabled && IsDue(settings) && Interlocked.CompareExchange(ref _running, 1, 0) == 0)
            {
                try { await ExecuteAsync(token); }
                finally { Volatile.Write(ref _running, 0); }
            }
        }
    }

    public async Task ExecuteAsync(CancellationToken token = default)
    {
        var settings = _settings.Current;
        for (var attempt = 0; attempt <= settings.MaxRetries; attempt++)
        {
            try
            {
                var newPath = await _renamer.RenameWithDateAsync(settings, token);
                settings.FolderPath = newPath; // retain a valid path for future recurring schedules.
                settings.LastRunAt = DateTimeOffset.Now;
                if (settings.Mode == ScheduleMode.Once) settings.IsEnabled = false;
                await _settings.SaveAsync(settings);
                await _logs.WriteAsync(LogLevel.Success, $"更名成功：{newPath}");
                return;
            }
            catch (Exception ex) when (attempt < settings.MaxRetries)
            {
                await _logs.WriteAsync(LogLevel.Error, $"第 {attempt + 1} 次执行失败：{ex.Message}；{settings.RetryDelaySeconds} 秒后重试。");
                await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, settings.RetryDelaySeconds)), token);
            }
            catch (Exception ex)
            {
                settings.LastRunAt = DateTimeOffset.Now;
                await _settings.SaveAsync(settings);
                await _logs.WriteAsync(LogLevel.Error, $"更名失败（已用尽 {settings.MaxRetries + 1} 次尝试）：{ex.Message}");
            }
        }
    }

    private static bool IsDue(ScheduleSettings settings)
    {
        var now = DateTimeOffset.Now;
        if (settings.Mode == ScheduleMode.Once) return !settings.LastRunAt.HasValue && now >= settings.ScheduledAt;
        return now >= settings.ScheduledAt && (!settings.LastRunAt.HasValue || now - settings.LastRunAt >= TimeSpan.FromMinutes(Math.Max(1, settings.IntervalMinutes)));
    }
}
