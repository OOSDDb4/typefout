using Typefout.App.ViewModels;
using Typefout.App.Views;

namespace Typefout.App.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void InlogButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OefeningenMenuPage());
    }
}