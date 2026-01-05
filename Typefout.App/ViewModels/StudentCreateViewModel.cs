using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

public partial class StudentCreateViewModel : ObservableObject
{
    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _passwordRepeat;

    [ObservableProperty]
    private ObservableCollection<Group> _groups;

    [ObservableProperty]
    private Group _selectedGroup;

    [RelayCommand]
    private async Task CreateStudent()
    {
        if (Password != PasswordRepeat)
        {
            return;
        }

        await Shell.Current.GoToAsync("..");
    }
}