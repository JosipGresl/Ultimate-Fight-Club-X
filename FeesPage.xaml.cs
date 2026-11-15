using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class FeesPage : ContentPage
{
    private readonly FeesViewModel _viewModel;

    public FeesPage(FeesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
    private async void OnPaymentClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: FeeItem item })
            await Shell.Current.GoToAsync($"addpayment?memberId={item.MemberId}");
    }
}