// LoginPageViewModel.cs (FULL UPDATED)
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.App.Views;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string _loginInput = string.Empty;

        [ObservableProperty]
        private string _passwordInput = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public LoginPageViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        public bool IsEmail(string input)
        {
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }

        [RelayCommand]
        private async Task Login()
        {
            User? authenticatedClient = null;

            if (IsEmail(LoginInput))
            {
                authenticatedClient = _authService.Login(null, LoginInput, PasswordInput);
            }
            else
            {
                authenticatedClient = _authService.Login(LoginInput, null, PasswordInput);
            }

            if (authenticatedClient == null)
            {
                _authService.CurrentUser = authenticatedClient;
                ErrorMessage = string.Empty;
                await Application.Current.MainPage.Navigation.PushAsync(new OefeningenMenuPage());
                ErrorMessage = "Ongeldige inloggegevens. Probeer het opnieuw.";
                return;
            }

            if (!authenticatedClient.IsActive)
            {
                ErrorMessage = "Dit account is gedeactiveerd.";
                return;
            }

            ErrorMessage = string.Empty;

            if (authenticatedClient.UserType == UserType.Admin)
            {
                await Shell.Current.GoToAsync("schools");
            }
            else if (authenticatedClient.UserType == UserType.Docent)
            {
                await Shell.Current.GoToAsync("teacherpage");
            }
            else
            {
                await Shell.Current.GoToAsync("oefeningen");
            }
        }
    }
}
