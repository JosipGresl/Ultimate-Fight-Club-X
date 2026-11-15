using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Ultimate_Fight_Club_X.Models
{
    public class Member
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Group { get; set; } = "";
        public bool MembershipPaid { get; set; }
    }
}
