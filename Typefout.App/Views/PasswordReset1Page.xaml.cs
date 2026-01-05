using Microsoft.Maui.ApplicationModel.Communication;
using Typefout.App.ViewModels;

namespace Typefout.App.Views;

public partial class PasswordReset1Page : ContentPage
{
    public PasswordReset1Page()
    {
        InitializeComponent();
    }

    public bool IsEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        try
        {
            System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(input);

            return addr.Address == input &&
                   addr.Host.Contains(".") &&
                   !addr.Host.EndsWith(".");
        }
        catch
        {
            return false;
        }
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        string userEmail = Email.Text;

        if (IsEmail(userEmail) is true)
        {
            Error.Text = string.Empty;
            await Shell.Current.GoToAsync($"{nameof(PasswordReset2Page)}?Email={userEmail}");
        }
        else
        {
            Error.Text = "voer een geldige mail in";
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        LoginPageViewModel vm = App.Services.GetRequiredService<LoginPageViewModel>();
        await Shell.Current.Navigation.PushAsync(new LoginPage(vm));
    }
}