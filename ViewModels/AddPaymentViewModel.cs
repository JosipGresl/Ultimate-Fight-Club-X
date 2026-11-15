using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Models;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class AddPaymentViewModel : ObservableObject, IQueryAttributable
{
    private readonly MemberDatabase _database;
    private int _memberId;

    public List<int> MonthOptions { get; } = new() { 1, 3, 6, 12 };

    [ObservableProperty] private string memberName = "";
    [ObservableProperty] private string amountText = "";
    [ObservableProperty] private DateTime paymentDate = DateTime.Today;
    [ObservableProperty] private int selectedMonths = 1;
    [ObservableProperty] private string errorMessage = "";

    public AddPaymentViewModel(MemberDatabase database)
    {
        _database = database;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("memberId", out var value))
        {
            _memberId = Convert.ToInt32(value);
            _ = LoadMemberAsync();
        }
    }

    private async Task LoadMemberAsync()
    {
        var member = await _database.GetMemberAsync(_memberId);
        MemberName = member?.FullName ?? "";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = "";

        var normalized = AmountText.Trim().Replace(',', '.');
        if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)
            || amount <= 0)
        {
            ErrorMessage = "Unesite ispravan iznos (veći od 0).";
            return;
        }

        // Ako članarina još vrijedi, novo razdoblje se nadovezuje na postojeće
        var latest = await _database.GetLatestValidUntilAsync(_memberId);
        var start = latest is not null && latest.Value.Date >= PaymentDate.Date
            ? latest.Value.Date
            : PaymentDate.Date;

        await _database.AddPaymentAsync(new Payment
        {
            MemberId = _memberId,
            Amount = amount,
            PaymentDate = PaymentDate.Date,
            ValidUntil = start.AddMonths(SelectedMonths)
        });

        await Shell.Current.GoToAsync("..");
    }
}