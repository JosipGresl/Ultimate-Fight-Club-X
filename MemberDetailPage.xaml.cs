using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class MemberDetailPage : ContentPage
{
    private readonly MemberDetailViewModel _viewModel;

    public MemberDetailPage(MemberDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}