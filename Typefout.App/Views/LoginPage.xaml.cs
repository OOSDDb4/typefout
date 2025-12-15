using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private async void InlogButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OefeningenMenuPage());
    }

    private void OnEyeIconTapped(object sender, EventArgs e)
    {
        PassWord.IsPassword = !PassWord.IsPassword;
    }
}