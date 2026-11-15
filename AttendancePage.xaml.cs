using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class AttendancePage : ContentPage
{
    private readonly AttendanceViewModel _viewModel;

    public AttendancePage(AttendanceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}