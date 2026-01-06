using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Helper;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(UserId), "userId")]
public partial class StudentEditViewModel : ObservableObject
{
    private readonly IUserRepo _userRepo;
    private readonly IGroupRepo _groupRepo;

    [ObservableProperty] private int _userId;
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _passwordRepeat = string.Empty;

    [ObservableProperty] private ObservableCollection<Group> _groups = new();
    [ObservableProperty] private Group? _selectedGroup;

    public StudentEditViewModel(IUserRepo userRepo, IGroupRepo groupRepo)
    {
        _userRepo = userRepo;
        _groupRepo = groupRepo;
    }

    partial void OnUserIdChanged(int value)
    {
        LoadStudentCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadStudent()
    {
        try
        {
            User? user = await _userRepo.GetByIdAsync(UserId);
            if (user == null)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Leerling niet gevonden.", "OK");
                return;
            }

            Username = user.Username;

            IEnumerable<Group> groups = await _groupRepo.GetBySchoolIdAsync(user.SchoolId);
            Groups.Clear();

            foreach (Group group in groups)
            {
                Groups.Add(group);
                if (user.GroupId == group.Id)
                {
                    SelectedGroup = group;
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task SaveStudent()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            await Application.Current.MainPage.DisplayAlert("Fout", "Gebruikersnaam is verplicht.", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(Password) && Password != PasswordRepeat)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", "Wachtwoorden komen niet overeen.", "OK");
            return;
        }

        try
        {
            User? user = await _userRepo.GetByIdAsync(UserId);
            if (user == null)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Leerling niet gevonden.", "OK");
                return;
            }

            user.Username = Username.Trim();
            user.GroupId = SelectedGroup != null ? SelectedGroup.Id : 0;

            if (!string.IsNullOrWhiteSpace(Password))
            {
                user.Password = PasswordHelper.HashPassword(Password);
            }

            await _userRepo.UpdateAsync(user);

            await Application.Current.MainPage.DisplayAlert("Gelukt", "Leerling is bijgewerkt.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
        }
    }
}
