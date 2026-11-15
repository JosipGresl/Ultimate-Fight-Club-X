using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Helpers;
using Ultimate_Fight_Club_X.Models;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class AddMemberViewModel : ObservableObject, IQueryAttributable
{
    private readonly MemberDatabase _database;
    private Member? _editing;

    public List<string> Categories { get; } = new() { "Redovni član", "Mlađi član", "Počasni član" };
    public List<string> Groups { get; } = ClubOptions.Groups;

    [ObservableProperty] private string pageTitle = "Novi član";
    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string oib = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsMinor))]
    private DateTime dateOfBirth = DateTime.Today.AddYears(-18);
    [ObservableProperty] private DateTime joinDate = DateTime.Today;
    [ObservableProperty] private string? selectedCategory;
    [ObservableProperty] private string? selectedGroup;
    [ObservableProperty] private string guardianName = "";
    [ObservableProperty] private bool guardianConsent;
    [ObservableProperty] private string errorMessage = "";

    public bool IsMinor => DateOfBirth > DateTime.Today.AddYears(-18);

    public AddMemberViewModel(MemberDatabase database)
    {
        _database = database;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("memberId", out var value))
            _ = LoadForEditAsync(Convert.ToInt32(value));
    }

    private async Task LoadForEditAsync(int id)
    {
        var member = await _database.GetMemberAsync(id);
        if (member is null) return;

        _editing = member;
        PageTitle = "Uredi člana";
        FullName = member.FullName;
        Oib = member.Oib;
        DateOfBirth = member.DateOfBirth;
        JoinDate = member.JoinDate;
        SelectedCategory = member.MembershipCategory;
        SelectedGroup = member.Group;
        GuardianName = member.GuardianName ?? "";
        GuardianConsent = member.GuardianConsent;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = "";

        if (string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "Unesite ime i prezime.";
            return;
        }
        if (!OibValidator.IsValid(Oib))
        {
            ErrorMessage = "OIB nije ispravan (mora imati 11 znamenki).";
            return;
        }
        if (DateOfBirth >= DateTime.Today)
        {
            ErrorMessage = "Datum rođenja mora biti u prošlosti.";
            return;
        }
        if (SelectedCategory is null || SelectedGroup is null)
        {
            ErrorMessage = "Odaberite kategoriju članstva i grupu.";
            return;
        }
        if (IsMinor && (string.IsNullOrWhiteSpace(GuardianName) || !GuardianConsent))
        {
            ErrorMessage = "Za maloljetnog člana potrebni su zakonski zastupnik i potvrda suglasnosti.";
            return;
        }

        var member = _editing ?? new Member();
        member.FullName = FullName.Trim();
        member.Oib = Oib;
        member.DateOfBirth = DateOfBirth;
        member.JoinDate = JoinDate;
        member.MembershipCategory = SelectedCategory;
        member.Group = SelectedGroup;
        member.GuardianName = IsMinor ? GuardianName.Trim() : null;
        member.GuardianConsent = IsMinor && GuardianConsent;

        if (_editing is null)
            await _database.AddMemberAsync(member);
        else
            await _database.UpdateMemberAsync(member);

        await Shell.Current.GoToAsync("..");
    }
}