using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Helpers;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class AttendanceViewModel : ObservableObject
{
    private readonly MemberDatabase _database;

    public List<string> Groups { get; } = ClubOptions.Groups;
    public ObservableCollection<AttendanceItem> Items { get; } = new();

    [ObservableProperty] private string? selectedGroup;
    [ObservableProperty] private DateTime selectedDate = DateTime.Today;
    [ObservableProperty] private string statusMessage = "";

    public AttendanceViewModel(MemberDatabase database)
    {
        _database = database;
    }

    partial void OnSelectedGroupChanged(string? value) => _ = LoadAsync();
    partial void OnSelectedDateChanged(DateTime value) => _ = LoadAsync();

    [RelayCommand]
    private async Task LoadAsync()
    {
        
        StatusMessage = "";
        if (SelectedGroup is null) return;

        var members = await _database.GetActiveMembersByGroupAsync(SelectedGroup);
        var present = await _database.GetPresentMemberIdsAsync(SelectedDate);
        Items.Clear();
        foreach (var m in members)
        {
            Items.Add(new AttendanceItem
            {
                MemberId = m.Id,
                FullName = m.FullName,
                IsPresent = present.Contains(m.Id)
            });
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        foreach (var item in Items)
            await _database.SetAttendanceAsync(item.MemberId, SelectedDate, item.IsPresent);

        StatusMessage = $"Spremljeno. Prisutno: {Items.Count(i => i.IsPresent)} od {Items.Count}.";
    }
}