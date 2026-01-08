using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class ResultsPage : ContentPage
{
    public ResultsPage(ResultsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void Back_Button(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("oefeningen");;
    }
}