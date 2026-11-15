using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class AddMemberPage : ContentPage
{
    public AddMemberPage(AddMemberViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}