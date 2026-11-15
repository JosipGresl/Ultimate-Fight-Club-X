using SQLite;

namespace Ultimate_Fight_Club_X.Models;

public class Attendance
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int MemberId { get; set; }

    public DateTime Date { get; set; }
}