using SQLite;

namespace Ultimate_Fight_Club_X.Models;

public class Member
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Oib { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime? LeaveDate { get; set; }
    public string MembershipCategory { get; set; } = "";
    public string Group { get; set; } = "";
    public string? GuardianName { get; set; }
    public bool GuardianConsent { get; set; }
    [Ignore]
    public bool HasLeft => LeaveDate is not null;

    [Ignore]
    public string Subtitle => HasLeft
        ? $"{Group} · napustio/la {LeaveDate:dd.MM.yyyy.}"
        : Group;
}