using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Models;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class MembersViewModel : ObservableObject
{
    private readonly MemberDatabase _database;

    public ObservableCollection<Member> Members { get; } = new();

    public MembersViewModel(MemberDatabase database)
    {
        _database = database;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Members.Clear();
        foreach (var member in await _database.GetMembersAsync())
            Members.Add(member);
    }

    [RelayCommand]
    private async Task AddMemberAsync()
    {
        await Shell.Current.GoToAsync("addmember");
    }
}