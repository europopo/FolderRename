using FolderRename.Models;

namespace FolderRename.Services;

/// <summary>Encapsulates filesystem mutations so alternative naming strategies can be added safely.</summary>
public sealed class RenameService
{
    public Task<string> RenameWithDateAsync(ScheduleSettings settings, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(settings.FolderPath) || !Directory.Exists(settings.FolderPath))
            throw new DirectoryNotFoundException("目标文件夹不存在或无法访问。");

        var source = new DirectoryInfo(settings.FolderPath);
        var parent = source.Parent ?? throw new InvalidOperationException("不能重命名磁盘根目录。");
        string desiredName;
        try { desiredName = DateTime.Now.ToString(settings.DateFormat); }
        catch (FormatException ex) { throw new InvalidOperationException("日期格式无效。", ex); }

        if (string.Equals(source.Name, desiredName, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(source.FullName);

        var destination = Path.Combine(parent.FullName, desiredName);
        if (Directory.Exists(destination))
            throw new IOException($"目标名称“{desiredName}”已存在。");

        Directory.Move(source.FullName, destination);
        return Task.FromResult(destination);
    }
}
