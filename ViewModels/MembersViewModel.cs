using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ultimate_Fight_Club_X.Models;

namespace Ultimate_Fight_Club_X.ViewModels;

public partial class MembersViewModel : ObservableObject
{
    public ObservableCollection<Member> Members { get; } = new();

    public MembersViewModel()
    {
        Members.Add(new Member { Id = 1, FullName = "Test Član 1", Group = "MMA - napredni" });
        Members.Add(new Member { Id = 2, FullName = "Test Član 2", Group = "BJJ - početnici" });
        Members.Add(new Member { Id = 3, FullName = "Test Član 3", Group = "Boks - djeca" });
    }

    [RelayCommand]
    private void AddTestMember()
    {
        var id = Members.Count + 1;
        Members.Add(new Member { Id = id, FullName = $"Test Član {id}", Group = "MMA - početnici" });
    }
}