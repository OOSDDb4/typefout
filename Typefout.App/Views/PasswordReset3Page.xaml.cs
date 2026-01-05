namespace Typefout.App.Views;

public partial class PasswordReset3Page : ContentPage
{
	public PasswordReset3Page()
	{
		InitializeComponent();
	}

    private async void OnDoorClicked(object sender, EventArgs e)
    {
        string pass1 = NewPasswordEntry.Text;
        string pass2 = RepeatPasswordEntry.Text;

        if (string.IsNullOrEmpty(pass1) || string.IsNullOrEmpty(pass2))
        {
            await DisplayAlert("Fout", "Vul beide wachtwoordvelden in.", "OK");
            return; 
        }

        if (pass1 != pass2)
        {
            await DisplayAlert("Fout", "De wachtwoorden komen niet overeen. Probeer het opnieuw.", "OK");

            RepeatPasswordEntry.Text = string.Empty;
            return;
        }

        await DisplayAlert("Succes", "Je wachtwoord is gewijzigd!", "Klaar");

        Application.Current.MainPage.Navigation.PushAsync(new MainPage());
    }
}