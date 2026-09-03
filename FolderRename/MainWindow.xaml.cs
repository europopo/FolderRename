using FolderRename.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FolderRename;

public sealed partial class MainWindow : Window
{
    public static MainWindow Current { get; private set; } = null!;
    public MainWindow()
    {
        InitializeComponent();
        Current = this;
        // Attach after InitializeComponent: the initially selected menu item can
        // raise SelectionChanged while XAML is still creating ContentFrame.
        RootNavigation.SelectionChanged += Navigation_SelectionChanged;
        ContentFrame.Navigate(typeof(SchedulerPage));
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (ContentFrame is null || args.SelectedItem is not NavigationViewItem item) return;
        ContentFrame.Navigate(item.Tag?.ToString() == "logs" ? typeof(LogPage) : typeof(SchedulerPage));
    }
}
