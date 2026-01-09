using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Helper;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;
using Windows.ApplicationModel.UserDataAccounts.SystemAccess;

namespace Typefout.App.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private int _schoolId;

        [ObservableProperty]
        private Group? _selectedGroup;

        [ObservableProperty]
        private IList<Group>? _groups;

        private readonly IUserRepo _userRepo;
        private readonly IGroupRepo _groupRepo;
        private readonly IAuthService _authService;
        public RegistrationViewModel(IUserRepo userRepo, IGroupRepo groupRepo, IAuthService authService)
        {
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _authService = authService;
            LoadGroups();
        }

        private async void LoadGroups()
        {
            IEnumerable<Group> groups = await _groupRepo.GetBySchoolIdAsync(_authService.CurrentUser.SchoolId);
            Groups = groups.ToList();
        }

        [RelayCommand]
        private async Task CreateStudent()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Gebruikersnaam is verplicht.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Wachtwoord is verplicht.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Wachtwoorden komen niet overeen.", "OK");
                return;
            }
            if (Password.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Wachtwoord moet minimaal 6 tekens lang zijn.", "OK");
                return;
            }

            User user = new User
            {
                Username = Username.Trim(),
                Password = PasswordHelper.HashPassword(Password),
                UserType = UserType.Leerling,
                SchoolId = _authService.CurrentUser.SchoolId,
                GroupId = SelectedGroup?.Id ?? 0,
                IsActive = true
            };

            try
            {
                await _userRepo.CreateAsync(user);
                await Application.Current.MainPage.DisplayAlert("Gelukt", "Leerling is aangemaakt.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
            }
        }
    }
}
