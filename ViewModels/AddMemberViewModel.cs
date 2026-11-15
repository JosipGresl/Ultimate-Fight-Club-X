using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Security.Cryptography;
using Ultimate_Fight_Club_X.Helpers;
using Ultimate_Fight_Club_X.Models;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class AddMemberViewModel : ObservableObject
{
    private readonly MemberDatabase _database;

    public List<string> Categories { get; } = new() { "Redovni član", "Mlađi član", "Počasni član" };
    public List<string> Groups { get; } = new() { "MMA - početnici", "MMA - napredni", "BJJ", "Boks", "Djeca" };

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

        var member = new Member
        {
            FullName = FullName.Trim(),
            Oib = Oib,
            DateOfBirth = DateOfBirth,
            JoinDate = JoinDate,
            MembershipCategory = SelectedCategory,
            Group = SelectedGroup,
            GuardianName = IsMinor ? GuardianName.Trim() : null,
            GuardianConsent = IsMinor && GuardianConsent
        };

        await _database.AddMemberAsync(member);
        await Shell.Current.GoToAsync("..");
    }
}