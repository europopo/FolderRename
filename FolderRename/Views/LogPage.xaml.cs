using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FolderRename.Views;

public sealed partial class LogPage : Page
{
    public LogPage()
    {
        InitializeComponent();
        DataContext = App.Services.Logs.Entries;
    }

    private async void Clear_Click(object sender, RoutedEventArgs e) => await App.Services.Logs.ClearAsync();
}
