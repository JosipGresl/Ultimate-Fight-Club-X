using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Models;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class MembersViewModel : ObservableObject
{
    private readonly MemberDatabase _database;
    private List<Member> _all = new();

    public ObservableCollection<Member> Members { get; } = new();

    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private bool showFormer;
    [ObservableProperty] private string countText = "";
    [ObservableProperty] private bool isEmpty;

    public MembersViewModel(MemberDatabase database)
    {
        _database = database;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnShowFormerChanged(bool value) => ApplyFilter();

    [RelayCommand]
    private async Task LoadAsync()
    {
        _all = await _database.GetMembersAsync();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchText.Trim();

        var filtered = _all
            .Where(m => ShowFormer || !m.HasLeft)
            .Where(m => query.Length == 0
                || m.FullName.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || m.Group.Contains(query, StringComparison.CurrentCultureIgnoreCase))
            .OrderBy(m => m.HasLeft)
            .ThenBy(m => m.FullName)
            .ToList();

        Members.Clear();
        foreach (var m in filtered)
            Members.Add(m);

        CountText = $"Prikazano: {filtered.Count}";
        IsEmpty = filtered.Count == 0;
    }

    [RelayCommand]
    private async Task AddMemberAsync()
    {
        await Shell.Current.GoToAsync("addmember");
    }
}