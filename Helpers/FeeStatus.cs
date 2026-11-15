namespace Ultimate_Fight_Club_X.Helpers;

public enum FeeState { Overdue = 0, NoPayment = 1, DueSoon = 2, Paid = 3 }

public static class FeeStatus
{
    public static FeeState Get(DateTime? validUntil, DateTime today)
    {
        if (validUntil is null) return FeeState.NoPayment;
        if (validUntil.Value.Date < today.Date) return FeeState.Overdue;
        if (validUntil.Value.Date <= today.Date.AddDays(7)) return FeeState.DueSoon;
        return FeeState.Paid;
    }
}