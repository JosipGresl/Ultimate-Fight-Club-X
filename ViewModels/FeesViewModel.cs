using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Helpers;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class FeesViewModel : ObservableObject
{
    private readonly MemberDatabase _database;

    public ObservableCollection<FeeItem> Items { get; } = new();

    [ObservableProperty] private bool onlyProblems;
    [ObservableProperty] private string summaryText = "";

    public FeesViewModel(MemberDatabase database)
    {
        _database = database;
    }

    partial void OnOnlyProblemsChanged(bool value) => _ = LoadAsync();

    [RelayCommand]
    private async Task LoadAsync()
    {
        var today = DateTime.Today;
        var members = (await _database.GetMembersAsync()).Where(m => m.LeaveDate is null);
        var latest = await _database.GetLatestValidUntilByMemberAsync();

        var all = members.Select(m =>
        {
            DateTime? validUntil = latest.TryGetValue(m.Id, out var d) ? d : null;
            var state = FeeStatus.Get(validUntil, today);

            var (text, color) = state switch
            {
                FeeState.Overdue => ($"Kasni (vrijedilo do {validUntil:dd.MM.yyyy.})", Color.FromArgb("#C62828")),
                FeeState.NoPayment => ("Nema evidentirane uplate", Color.FromArgb("#C62828")),
                FeeState.DueSoon => ($"Dospijeva {validUntil:dd.MM.yyyy.}", Color.FromArgb("#EF6C00")),
                _ => ($"Plaćeno do {validUntil:dd.MM.yyyy.}", Color.FromArgb("#2E7D32"))
            };

            return new FeeItem
            {
                MemberId = m.Id,
                FullName = m.FullName,
                Group = m.Group,
                State = state,
                StatusText = text,
                StatusColor = color
            };
        }).OrderBy(i => i.State).ThenBy(i => i.FullName).ToList();

        SummaryText = $"Kasni: {all.Count(i => i.State == FeeState.Overdue)}  ·  " +
                      $"Bez uplate: {all.Count(i => i.State == FeeState.NoPayment)}  ·  " +
                      $"Dospijeva uskoro: {all.Count(i => i.State == FeeState.DueSoon)}";

        Items.Clear();
        foreach (var item in all.Where(i => !OnlyProblems ||
                     i.State is FeeState.Overdue or FeeState.NoPayment))
            Items.Add(item);
    }

}