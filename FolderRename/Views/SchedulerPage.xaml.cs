using FolderRename.Helpers;
using FolderRename.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FolderRename.Views;

public sealed partial class SchedulerPage : Page
{
    private readonly SchedulerViewModel _viewModel;

    public SchedulerPage()
    {
        InitializeComponent();
        _viewModel = new SchedulerViewModel(App.Services.Settings, App.Services.Scheduler, App.Services.Logs);
        DataContext = _viewModel;
        RunDatePicker.Date = _viewModel.ScheduledAt;
        RunTimePicker.Time = _viewModel.ScheduledAt.TimeOfDay;
    }

    private async void Browse_Click(object sender, RoutedEventArgs e)
    {
        var path = await FolderPickerHelper.PickAsync(MainWindow.Current);
        if (path is not null) _viewModel.FolderPath = path;
    }

    private async void Save_Click(object sender, RoutedEventArgs e) => await _viewModel.SaveAsync();

    private void RunDatePicker_DateChanged(DatePicker sender, DatePickerValueChangedEventArgs args)
        => _viewModel.ScheduledAt = new DateTimeOffset(args.NewDate.Date + RunTimePicker.Time, TimeZoneInfo.Local.GetUtcOffset(args.NewDate.Date));

    private void RunTimePicker_TimeChanged(TimePicker sender, TimePickerValueChangedEventArgs args)
    {
        var date = RunDatePicker.Date.Date;
        _viewModel.ScheduledAt = new DateTimeOffset(date + args.NewTime, TimeZoneInfo.Local.GetUtcOffset(date));
    }
}
