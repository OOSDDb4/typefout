namespace Typefout.App.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
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