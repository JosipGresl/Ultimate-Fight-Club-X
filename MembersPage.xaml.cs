using Ultimate_Fight_Club_X.Models;
using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class MembersPage : ContentPage
{
	public MembersPage()
	{
        InitializeComponent();
        BindingContext = new MembersViewModel();
    }
}