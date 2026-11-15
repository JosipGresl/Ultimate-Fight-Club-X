using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Ultimate_Fight_Club_X.Helpers;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class MemberDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly MemberDatabase _database;
    private int _memberId;

    public ObservableCollection<string> Payments { get; } = new();

    [ObservableProperty] private string fullName = "";
    [ObservableProperty] private string groupName = "";
    [ObservableProperty] private string categoryName = "";
    [ObservableProperty] private string maskedOib = "";
    [ObservableProperty] private string dateOfBirthText = "";
    [ObservableProperty] private string joinDateText = "";
    [ObservableProperty] private string statusText = "";
    [ObservableProperty] private Color statusColor = Colors.Gray;
    [ObservableProperty] private string attendanceText = "";
    [ObservableProperty] private string guardianText = "";
    [ObservableProperty] private bool hasGuardian;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsFormer))]
    private bool isActive = true;

    public bool IsFormer => !IsActive;
    [ObservableProperty] private bool noPayments;

    public MemberDetailViewModel(MemberDatabase database)
    {
        _database = database;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("memberId", out var value))
        {
            _memberId = Convert.ToInt32(value);
            _ = LoadAsync();
        }
    }

    public async Task LoadAsync()
    {
        if (_memberId == 0) return;

        var member = await _database.GetMemberAsync(_memberId);
        if (member is null) return;

        FullName = member.FullName;
        GroupName = member.Group;
        CategoryName = member.MembershipCategory;
        MaskedOib = member.Oib.Length == 11 ? new string('•', 7) + member.Oib[7..] : "";
        DateOfBirthText = member.DateOfBirth.ToString("dd.MM.yyyy.");
        JoinDateText = member.JoinDate.ToString("dd.MM.yyyy.");
        IsActive = member.LeaveDate is null;

        HasGuardian = !string.IsNullOrWhiteSpace(member.GuardianName);
        GuardianText = HasGuardian
            ? $"Zakonski zastupnik: {member.GuardianName} (suglasnost: {(member.GuardianConsent ? "da" : "ne")})"
            : "";

        if (!IsActive)
        {
            StatusText = $"Članstvo završeno {member.LeaveDate:dd.MM.yyyy.}";
            StatusColor = Colors.Gray;
        }
        else
        {
            var latest = await _database.GetLatestValidUntilAsync(_memberId);
            var state = FeeStatus.Get(latest, DateTime.Today);
            var (text, color) = state switch
            {
                FeeState.Overdue => ($"Članarina kasni (vrijedila do {latest:dd.MM.yyyy.})", Color.FromArgb("#C62828")),
                FeeState.NoPayment => ("Nema evidentirane uplate", Color.FromArgb("#C62828")),
                FeeState.DueSoon => ($"Članarina dospijeva {latest:dd.MM.yyyy.}", Color.FromArgb("#EF6C00")),
                _ => ($"Plaćeno do {latest:dd.MM.yyyy.}", Color.FromArgb("#2E7D32"))
            };
            StatusText = text;
            StatusColor = color;
        }

        var dates = await _database.GetAttendanceDatesAsync(_memberId);
        if (dates.Count == 0)
        {
            AttendanceText = "Nema evidentiranih dolazaka.";
        }
        else
        {
            var last30 = dates.Count(d => d >= DateTime.Today.AddDays(-30));
            AttendanceText = $"Ukupno: {dates.Count}  ·  zadnjih 30 dana: {last30}  ·  zadnji dolazak: {dates.Max():dd.MM.yyyy.}";
        }

        
        var payments = await _database.GetPaymentsAsync(_memberId);
        Payments.Clear();
        foreach (var p in payments)
            Payments.Add($"{p.PaymentDate:dd.MM.yyyy.}  ·  {p.Amount:0.00} EUR  ·  vrijedi do {p.ValidUntil:dd.MM.yyyy.}");
        NoPayments = payments.Count == 0;
    }

    [RelayCommand]
    private async Task EditAsync()
    {
        await Shell.Current.GoToAsync($"addmember?memberId={_memberId}");
    }

    [RelayCommand]
    private async Task AddPaymentAsync()
    {
        await Shell.Current.GoToAsync($"addpayment?memberId={_memberId}");
    }

    [RelayCommand]
    private async Task EndMembershipAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Završi članstvo",
            $"Označiti da {FullName} više nije član? Dolasci i uplate ostaju sačuvani.",
            "Da", "Odustani");
        if (!confirm) return;

        var member = await _database.GetMemberAsync(_memberId);
        if (member is null) return;

        member.LeaveDate = DateTime.Today;
        await _database.UpdateMemberAsync(member);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Trajno brisanje",
            $"Trajno obrisati člana {FullName} zajedno sa svim uplatama i dolascima? Ovo se ne može poništiti.",
            "Obriši", "Odustani");
        if (!confirm) return;

        await _database.DeleteMemberAsync(_memberId);
        await Shell.Current.GoToAsync("..");
    }
    [RelayCommand]
    private async Task ReactivateAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Ponovna aktivacija",
            $"Vratiti {FullName} u aktivne članove?",
            "Da", "Odustani");
        if (!confirm) return;

        var member = await _database.GetMemberAsync(_memberId);
        if (member is null) return;

        member.LeaveDate = null;
        await _database.UpdateMemberAsync(member);
        await LoadAsync();
    }
}