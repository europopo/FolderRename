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

    /// <summary>Shows an actionable error instead of silently terminating the process.</summary>
    public void ShowError(string message)
    {
        var content = new StackPanel
        {
            Spacing = 12
        };
        content.Children.Add(new TextBlock { Text = "应用启动时遇到问题", FontSize = 24, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });
        content.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap });
        Content = new Border { Padding = new Thickness(32), Child = content };
    }
}
