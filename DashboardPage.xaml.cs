using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnInactiveTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Element { BindingContext: InactiveItem item })
            await Shell.Current.GoToAsync($"memberdetail?memberId={item.MemberId}");
    }
}