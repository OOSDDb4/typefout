using Typefout.App.ViewModels;

namespace Typefout.App.Views.Admin;

public partial class SchoolInfoPage : ContentPage
{
    private readonly SchoolInfoViewModel _viewModel;

    public SchoolInfoPage(SchoolInfoViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadDataCommand.Execute(null);
    }
}