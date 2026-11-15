using CommunityToolkit.Mvvm.ComponentModel;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class AttendanceItem : ObservableObject
{
    public int MemberId { get; init; }
    public string FullName { get; init; } = "";

    [ObservableProperty]
    private bool isPresent;
}