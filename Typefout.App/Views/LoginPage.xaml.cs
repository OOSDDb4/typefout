using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }


    private void OnEyeIconTapped(object sender, EventArgs e)
    {
        PassWord.IsPassword = !PassWord.IsPassword;
    }
}