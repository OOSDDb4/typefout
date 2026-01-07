using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Data.Repo;
using Typefout.Core.Helper;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;
using Typefout.Core.Services;
using Group = Typefout.Core.Models.Group;

namespace Typefout.App.ViewModels
{
    public partial class RegistrationViewModel : BindableObject
    {
        private readonly IUserRepo _userRepo;
        private readonly IGroupRepo _groupRepo;
        private readonly IAuthService _auth;

        private ObservableCollection<Group> _groups;

        private string _username;
        private string _password;
        private string _confirmPassword;
        private Group _selectedGroup;
        private bool _isPasswordHidden = true;

        public ObservableCollection<Group> Groups { get => _groups; set { _groups = value; OnPropertyChanged(); } }

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }
        public Group SelectedGroup { get => _selectedGroup; set { _selectedGroup = value; OnPropertyChanged(); } }
        public bool IsPasswordHidden { get => _isPasswordHidden; set { _isPasswordHidden = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand TogglePasswordCommand { get; }

        public RegistrationViewModel(IUserRepo userRepo, IGroupRepo groupRepo, IAuthService authService)
        {
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _auth = authService;

            _groups = new ObservableCollection<Group>();
            LoadGroups().Wait();

            RegisterCommand = new Command(OnRegister);
            TogglePasswordCommand = new Command(() => IsPasswordHidden = !IsPasswordHidden);
        }

        private async void OnRegister()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Vul alle velden in.", "OK");
                return;
            }

            if (Password.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Het wachtwoord moet minimaal 6 tekens zijn", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "De wachtwoorden komen niet overeen. Controleer ze nog een keer", "OK");
                return;
            }

            if (_userRepo.GetUser(Username) != null)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "De gebruikersnaam is al in gebruik.", "OK");
                return;
            }

            User user = new User
            {
                Username = Username,
                Email = string.Empty,
                Password = PasswordHelper.HashPassword(Password),
                UserType = UserType.Leerling,
                SchoolId = _auth.CurrentUser.SchoolId,
                GroupId = SelectedGroup.Id
            };
            await _userRepo.CreateAsync(user);
            await Application.Current.MainPage.DisplayAlert("Succes", $"Gebruiker {Username} aangemaakt!", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        [RelayCommand]
        private async Task LoadGroups()
        {
            if (_auth.CurrentUser is null)
            {
                return;
            }
            User currentUser = _auth.CurrentUser;
            IEnumerable<Group> groups = await _groupRepo.GetBySchoolIdAsync(_auth.CurrentUser.SchoolId);

            Groups.Clear();

            foreach (Group group in groups)
            {
                Groups.Add(group);
            }
        }
    }
}