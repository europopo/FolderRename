using Windows.Storage.Pickers;
using WinRT.Interop;

namespace FolderRename.Helpers;

public static class FolderPickerHelper
{
    public static async Task<string?> PickAsync(Microsoft.UI.Xaml.Window window)
    {
        var picker = new FolderPicker();
        picker.FileTypeFilter.Add("*");
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window));
        var folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }
}
