using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Helper;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(UserId), "userId")]
public partial class TeacherEditViewModel : ObservableObject
{
    private readonly IUserRepo _userRepo;

    [ObservableProperty] private int _userId;
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _passwordRepeat = string.Empty;

    public TeacherEditViewModel(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    partial void OnUserIdChanged(int value)
    {
        LoadTeacherCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadTeacher()
    {
        User? user = await _userRepo.GetByIdAsync(UserId);
        if (user == null) return;

        Username = user.Username;
        Email = user.Email;
    }

    [RelayCommand]
    private async Task SaveTeacher()
    {
        if (string.IsNullOrWhiteSpace(Username)) return;
        if (!string.IsNullOrEmpty(Password) && Password != PasswordRepeat) return;

        User user = await _userRepo.GetByIdAsync(UserId) ?? new User();
        user.Username = Username;
        user.Email = Email;

        if (!string.IsNullOrWhiteSpace(Password))
        {
            user.Password = PasswordHelper.HashPassword(Password);
        }

        await _userRepo.UpdateAsync(user);
        await Shell.Current.GoToAsync("..");
    }
}