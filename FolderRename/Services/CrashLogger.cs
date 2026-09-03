namespace FolderRename.Services;

/// <summary>
/// Writes startup and unhandled-exception diagnostics to a location that is
/// available for unpackaged WinUI 3 deployments as well as Visual Studio runs.
/// </summary>
public static class CrashLogger
{
    private static readonly object SyncRoot = new();

    public static string LogDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FolderRename");

    public static string LogFilePath => Path.Combine(LogDirectory, "crash.log");

    public static void Write(string title, Exception? exception = null)
    {
        try
        {
            lock (SyncRoot)
            {
                Directory.CreateDirectory(LogDirectory);

                var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {title}{Environment.NewLine}";
                if (exception is not null)
                {
                    text += exception + Environment.NewLine;
                }

                text += $"OS: {Environment.OSVersion}{Environment.NewLine}";
                text += $"Process: {Environment.ProcessPath}{Environment.NewLine}";
                text += $"BaseDirectory: {AppContext.BaseDirectory}{Environment.NewLine}";
                text += $"64-bit process: {Environment.Is64BitProcess}{Environment.NewLine}";
                text += $"Runtime: {Environment.Version}{Environment.NewLine}";
                text += Environment.NewLine;

                File.AppendAllText(LogFilePath, text);
            }
        }
        catch
        {
            // Diagnostics must never become another source of application failure.
        }
    }
}
