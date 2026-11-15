using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class AddPaymentPage : ContentPage
{
    public AddPaymentPage(AddPaymentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}