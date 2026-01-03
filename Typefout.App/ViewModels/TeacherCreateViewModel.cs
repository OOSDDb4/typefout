using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Typefout.App.ViewModels;

public partial class TeacherCreateViewModel : ObservableObject
{
    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _passwordRepeat;

    [RelayCommand]
    private async Task CreateTeacher()
    {
        if (Password != PasswordRepeat)
        {
            return;
        }

        await Shell.Current.GoToAsync("..");
    }
}