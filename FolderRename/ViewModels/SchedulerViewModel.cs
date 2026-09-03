using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderRename.Models;
using FolderRename.Services;

namespace FolderRename.ViewModels;

public partial class SchedulerViewModel : ObservableObject
{
    private readonly SettingsStore _settings;
    private readonly SchedulerService _scheduler;
    private readonly LogService _logs;

    [ObservableProperty] private string folderPath = string.Empty;
    [ObservableProperty] private string renamePrefix = string.Empty;
    [ObservableProperty] private string dateFormat = "yyyyMMdd";
    [ObservableProperty] private ScheduleMode mode;
    [ObservableProperty] private DateTimeOffset scheduledAt;
    [ObservableProperty] private RecurrenceOption? selectedRecurrence;
    [ObservableProperty] private int retryDelaySeconds;
    [ObservableProperty] private int maxRetries;
    [ObservableProperty] private bool isEnabled;
    [ObservableProperty] private string statusText = "尚未启用计划";

    public Array ScheduleModes { get; } = Enum.GetValues(typeof(ScheduleMode));
    public IReadOnlyList<RecurrenceOption> RecurrenceOptions { get; } =
    [
        new(RecurrenceType.Daily, "每天（同一时间）"),
        new(RecurrenceType.Weekly, "每周（同一星期与时间）"),
        new(RecurrenceType.Monthly, "每月（同一日期与时间）"),
        new(RecurrenceType.Yearly, "每年（同一月日与时间）")
    ];

    public SchedulerViewModel(SettingsStore settings, SchedulerService scheduler, LogService logs)
    {
        _settings = settings; _scheduler = scheduler; _logs = logs;
        Load(settings.Current);
    }

    public async Task SaveAsync()
    {
        var folderPath = FolderPath.Trim();
        var prefix = RenamePrefix.Trim();
        if (string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(folderPath))
            prefix = Path.GetFileName(Path.TrimEndingDirectorySeparator(folderPath));

        var recurrence = SelectedRecurrence?.Type ?? RecurrenceType.Weekly;
        var value = new ScheduleSettings { FolderPath = folderPath, RenamePrefix = prefix, DateFormat = DateFormat.Trim(), Mode = Mode, Recurrence = recurrence, ScheduledAt = ScheduledAt, RetryDelaySeconds = Math.Max(1, RetryDelaySeconds), MaxRetries = Math.Max(0, MaxRetries), IsEnabled = IsEnabled, LastRunAt = _settings.Current.LastRunAt };
        RenamePrefix = prefix;
        await _settings.SaveAsync(value);
        StatusText = IsEnabled ? "计划已保存并启用" : "计划已保存（未启用）";
        await _logs.WriteAsync(LogLevel.Information, StatusText);
    }

    [RelayCommand]
    private async Task RunNowAsync()
    {
        await SaveAsync();
        StatusText = "正在立即执行…";
        await _scheduler.ExecuteAsync();
        StatusText = "立即执行已完成，请查看日志";
    }

    private void Load(ScheduleSettings value)
    {
        FolderPath = value.FolderPath; RenamePrefix = value.RenamePrefix; DateFormat = value.DateFormat; Mode = value.Mode; ScheduledAt = value.ScheduledAt;
        SelectedRecurrence = RecurrenceOptions.First(option => option.Type == value.Recurrence);
        RetryDelaySeconds = value.RetryDelaySeconds; MaxRetries = value.MaxRetries; IsEnabled = value.IsEnabled;
        StatusText = IsEnabled ? "计划正在后台监控" : "尚未启用计划";
    }
}

public sealed record RecurrenceOption(RecurrenceType Type, string DisplayName);
