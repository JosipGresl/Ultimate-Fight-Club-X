using Ultimate_Fight_Club_X.Helpers;

namespace Ultimate_Fight_Club_X.ViewModels;

public class FeeItem
{
    public int MemberId { get; init; }
    public string FullName { get; init; } = "";
    public string Group { get; init; } = "";
    public FeeState State { get; init; }
    public string StatusText { get; init; } = "";
    public Color StatusColor { get; init; } = Colors.Gray;
}