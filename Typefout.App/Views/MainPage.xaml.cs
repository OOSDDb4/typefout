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
        LoginPageViewModel vm = App.Services.GetRequiredService<LoginPageViewModel>();
        await Shell.Current.Navigation.PushAsync(new LoginPage(vm));
    }
}