using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Helpers;
using Ultimate_Fight_Club_X.Services;

namespace Ultimate_Fight_Club_X.ViewModels;

public class BarItem
{
    public string Label { get; init; } = "";
    public string Value { get; init; } = "";
    public double Progress { get; init; }
}

public class InactiveItem
{
    public int MemberId { get; init; }
    public string FullName { get; init; } = "";
    public string Group { get; init; } = "";
    public string LastSeenText { get; init; } = "";
}

public partial class DashboardViewModel : ObservableObject
{
    private readonly MemberDatabase _database;

    public ObservableCollection<BarItem> RevenueBars { get; } = new();
    public ObservableCollection<BarItem> GroupBars { get; } = new();
    public ObservableCollection<InactiveItem> InactiveMembers { get; } = new();

    [ObservableProperty] private string activeMembersText = "0";
    [ObservableProperty] private string newThisMonthText = "0";
    [ObservableProperty] private string overdueText = "0";
    [ObservableProperty] private string revenueThisMonthText = "0.00 EUR";
    [ObservableProperty] private bool hasInactive;

    public DashboardViewModel(MemberDatabase database)
    {
        _database = database;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var today = DateTime.Today;
        var since = today.AddDays(-30);

        var members = (await _database.GetMembersAsync()).Where(m => m.LeaveDate is null).ToList();
        var payments = await _database.GetAllPaymentsAsync();
        var attendance = await _database.GetAllAttendanceAsync();
        var latest = await _database.GetLatestValidUntilByMemberAsync();

        // --- Sažetak ---
        ActiveMembersText = members.Count.ToString();
        NewThisMonthText = members.Count(m => m.JoinDate.Year == today.Year && m.JoinDate.Month == today.Month).ToString();

        int overdue = members.Count(m =>
        {
            DateTime? validUntil = latest.TryGetValue(m.Id, out var d) ? d : null;
            return FeeStatus.Get(validUntil, today) is FeeState.Overdue or FeeState.NoPayment;
        });
        OverdueText = overdue.ToString();

        var thisMonth = payments
            .Where(p => p.PaymentDate.Year == today.Year && p.PaymentDate.Month == today.Month)
            .Sum(p => p.Amount);
        RevenueThisMonthText = $"{thisMonth:0.00} EUR";

        // --- Prihod po mjesecima (zadnjih 6) ---
        RevenueBars.Clear();
        var firstOfMonth = new DateTime(today.Year, today.Month, 1);
        var monthly = Enumerable.Range(0, 6)
            .Select(i => firstOfMonth.AddMonths(-(5 - i)))
            .Select(month => new
            {
                Month = month,
                Sum = payments
                    .Where(p => p.PaymentDate.Year == month.Year && p.PaymentDate.Month == month.Month)
                    .Sum(p => p.Amount)
            })
            .ToList();

        var maxRevenue = Math.Max(monthly.Max(x => x.Sum), 1m);
        foreach (var x in monthly)
        {
            RevenueBars.Add(new BarItem
            {
                Label = x.Month.ToString("MM.yyyy."),
                Value = $"{x.Sum:0.00} EUR",
                Progress = (double)(x.Sum / maxRevenue)
            });
        }

        // --- Popunjenost grupa (zadnjih 30 dana) ---
        GroupBars.Clear();
        var groupOf = members.ToDictionary(m => m.Id, m => m.Group);
        var recent = attendance
            .Where(a => a.Date >= since && groupOf.ContainsKey(a.MemberId))
            .ToList();

        var groupStats = ClubOptions.Groups.Select(g =>
        {
            var records = recent.Where(a => groupOf[a.MemberId] == g).ToList();
            var sessions = records.Select(a => a.Date.Date).Distinct().Count();
            var avg = sessions == 0 ? 0 : records.Count / (double)sessions;
            return new { Group = g, Sessions = sessions, Avg = avg, Members = members.Count(m => m.Group == g) };
        }).ToList();

        var maxAvg = Math.Max(groupStats.Max(s => s.Avg), 1);
        foreach (var s in groupStats)
        {
            GroupBars.Add(new BarItem
            {
                Label = $"{s.Group} ({s.Members} članova)",
                Value = s.Sessions == 0 ? "nema podataka" : $"{s.Avg:0.0} po treningu ({s.Sessions} treninga)",
                Progress = s.Avg / maxAvg
            });
        }

        // --- Neaktivni članovi (30 dana bez dolaska) ---
        InactiveMembers.Clear();
        var lastSeen = attendance
            .GroupBy(a => a.MemberId)
            .ToDictionary(g => g.Key, g => g.Max(a => a.Date));

        var inactive = members
            .Where(m => m.JoinDate.Date <= since &&
                        (!lastSeen.TryGetValue(m.Id, out var d) || d < since))
            .OrderBy(m => lastSeen.TryGetValue(m.Id, out var d) ? d : DateTime.MinValue)
            .ToList();

        foreach (var m in inactive)
        {
            InactiveMembers.Add(new InactiveItem
            {
                MemberId = m.Id,
                FullName = m.FullName,
                Group = m.Group,
                LastSeenText = lastSeen.TryGetValue(m.Id, out var d)
                    ? $"zadnji dolazak {d:dd.MM.yyyy.}"
                    : "nema evidentiranih dolazaka"
            });
        }
        HasInactive = InactiveMembers.Count > 0;
    }
}