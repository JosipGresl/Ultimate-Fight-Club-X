using SQLite;

namespace Ultimate_Fight_Club_X.Models;

public class Payment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int MemberId { get; set; }

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime ValidUntil { get; set; }
}