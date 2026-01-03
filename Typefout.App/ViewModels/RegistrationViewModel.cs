using System.Windows.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Services;

namespace Typefout.App.ViewModels
{
    public class RegistrationViewModel : BindableObject
    {
        private readonly IAuthService _authService;
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _selectedGroup;
        private bool _isPasswordHidden = true;

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }
        public string SelectedGroup { get => _selectedGroup; set { _selectedGroup = value; OnPropertyChanged(); } }
        public bool IsPasswordHidden { get => _isPasswordHidden; set { _isPasswordHidden = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand TogglePasswordCommand { get; }

        public RegistrationViewModel(IAuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(OnRegister);
            TogglePasswordCommand = new Command(() => IsPasswordHidden = !IsPasswordHidden);
        }

        private async void OnRegister()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Vul alle velden in.", "OK");
                return;
            }

            if (Password.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Wachtwoord te kort (min. 6).", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", $"Wachtwoorden zijn niet gelijk.", "OK");
                return;
            }
            _authService.Register(Username, null, Password, SelectedGroup);

            await Application.Current.MainPage.DisplayAlert("Succes", $"Gebruiker {Username} aangemaakt!", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}