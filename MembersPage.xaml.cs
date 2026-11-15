using Ultimate_Fight_Club_X.Models;

namespace Ultimate_Fight_Club_X;

public partial class MembersPage : ContentPage
{
	public MembersPage()
	{
        InitializeComponent();
        MembersList.ItemsSource = new List<Member>
{
    new() { Id = 1, FullName = "Test Član 1", Group = "MMA - napredni" },
    new() { Id = 2, FullName = "Test Član 2", Group = "BJJ - početnici" },
    new() { Id = 3, FullName = "Test Član 3", Group = "Boks - djeca" },
};
    }
}