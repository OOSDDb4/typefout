using Typefout.App.ViewModels;

namespace Typefout.App.Views.Admin;

public partial class SchoolsPage : ContentPage
{
    private readonly SchoolsViewModel _viewModel;

    public SchoolsPage(SchoolsViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSchoolsCommand.ExecuteAsync(null);
    }
}