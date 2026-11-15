using Ultimate_Fight_Club_X.Models;
using Ultimate_Fight_Club_X.ViewModels;

namespace Ultimate_Fight_Club_X;

public partial class MembersPage : ContentPage
{
    private readonly MembersViewModel _viewModel;

    public MembersPage(MembersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }

    private async void OnMemberSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Member member)
        {
            ((CollectionView)sender!).SelectedItem = null;
            await Shell.Current.GoToAsync($"memberdetail?memberId={member.Id}");
        }
    }
}