namespace FolderRename.Models;

public enum ScheduleMode { Once, Recurring }

public sealed class ScheduleSettings
{
    public string FolderPath { get; set; } = string.Empty;
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public ScheduleMode Mode { get; set; } = ScheduleMode.Once;
    public DateTimeOffset ScheduledAt { get; set; } = DateTimeOffset.Now.AddMinutes(5);
    public int IntervalMinutes { get; set; } = 60;
    public int RetryDelaySeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public bool IsEnabled { get; set; }
    public DateTimeOffset? LastRunAt { get; set; }
}

public sealed class LogEntry
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
}

public enum LogLevel { Information, Success, Error }
