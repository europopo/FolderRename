using FolderRename.Helpers;
using FolderRename.Models;
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
        RecurringTimePicker.Time = _viewModel.ScheduledAt.TimeOfDay;
        WeekdaySelector.SelectedIndex = (int)_viewModel.ScheduledAt.DayOfWeek;
        MonthDayBox.Value = _viewModel.ScheduledAt.Day;
        MonthSelector.SelectedIndex = _viewModel.ScheduledAt.Month - 1;
        YearDayBox.Value = _viewModel.ScheduledAt.Day;
        RecurrenceButtons.SelectedIndex = (int)(_viewModel.SelectedRecurrence?.Type ?? RecurrenceType.Daily);
        UpdateScheduleMode();
    }

    private async void Browse_Click(object sender, RoutedEventArgs e)
    {
        var path = await FolderPickerHelper.PickAsync(MainWindow.Current);
        if (path is not null)
        {
            _viewModel.FolderPath = path;
            if (string.IsNullOrWhiteSpace(_viewModel.RenamePrefix))
                _viewModel.RenamePrefix = Path.GetFileName(Path.TrimEndingDirectorySeparator(path));
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e) => await _viewModel.SaveAsync();

    private void RunDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs args)
    {
        if (_viewModel is null) return;
        _viewModel.ScheduledAt = new DateTimeOffset(args.NewDate.Date + RunTimePicker.Time, TimeZoneInfo.Local.GetUtcOffset(args.NewDate.Date));
    }

    private void RunTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs args)
    {
        if (_viewModel is null) return;
        var date = RunDatePicker.Date.Date;
        _viewModel.ScheduledAt = new DateTimeOffset(date + args.NewTime, TimeZoneInfo.Local.GetUtcOffset(date));
    }

    private void ModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateScheduleMode();

    private void RecurrenceButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel is null || RecurrenceButtons.SelectedIndex < 0) return;
        _viewModel.SelectedRecurrence = _viewModel.RecurrenceOptions[RecurrenceButtons.SelectedIndex];
        UpdateRecurrenceDetails();
    }

    private void RecurringTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs args)
    {
        if (_viewModel is null) return;
        UpdateScheduledDate(_viewModel.ScheduledAt.LocalDateTime.Date, args.NewTime);
    }

    private void WeekdaySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel is null || WeekdaySelector.SelectedIndex < 0) return;
        var date = _viewModel.ScheduledAt.LocalDateTime.Date;
        var offset = (WeekdaySelector.SelectedIndex - (int)date.DayOfWeek + 7) % 7;
        UpdateScheduledDate(date.AddDays(offset), RecurringTimePicker.Time);
    }

    private void MonthDayBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (_viewModel is null || double.IsNaN(args.NewValue)) return;
        var current = _viewModel.ScheduledAt.LocalDateTime;
        UpdateScheduledDate(CreateDate(current.Year, current.Month, (int)args.NewValue), RecurringTimePicker.Time);
    }

    private void MonthSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel is null || MonthSelector.SelectedIndex < 0) return;
        var current = _viewModel.ScheduledAt.LocalDateTime;
        UpdateScheduledDate(CreateDate(current.Year, MonthSelector.SelectedIndex + 1, (int)YearDayBox.Value), RecurringTimePicker.Time);
    }

    private void YearDayBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (_viewModel is null || double.IsNaN(args.NewValue)) return;
        var current = _viewModel.ScheduledAt.LocalDateTime;
        UpdateScheduledDate(CreateDate(current.Year, current.Month, (int)args.NewValue), RecurringTimePicker.Time);
    }

    private void UpdateScheduleMode()
    {
        if (_viewModel is null) return;
        var recurring = _viewModel.Mode == ScheduleMode.Recurring;
        OnceSchedulePanel.Visibility = recurring ? Visibility.Collapsed : Visibility.Visible;
        RecurringSchedulePanel.Visibility = recurring ? Visibility.Visible : Visibility.Collapsed;
        if (recurring) UpdateRecurrenceDetails();
    }

    private void UpdateRecurrenceDetails()
    {
        if (_viewModel is null) return;
        var type = _viewModel.SelectedRecurrence?.Type ?? RecurrenceType.Daily;
        WeekdaySelector.Visibility = type == RecurrenceType.Weekly ? Visibility.Visible : Visibility.Collapsed;
        MonthDayBox.Visibility = type == RecurrenceType.Monthly ? Visibility.Visible : Visibility.Collapsed;
        YearDatePanel.Visibility = type == RecurrenceType.Yearly ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateScheduledDate(DateTime date, TimeSpan time)
        => _viewModel.ScheduledAt = new DateTimeOffset(date + time, TimeZoneInfo.Local.GetUtcOffset(date));

    private static DateTime CreateDate(int year, int month, int requestedDay)
        => new(year, month, Math.Clamp(requestedDay, 1, DateTime.DaysInMonth(year, month)));
}
