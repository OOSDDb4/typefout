using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(SchoolId), "schoolId")]
public partial class GroupCreateViewModel : ObservableObject
{
    private readonly IGroupRepo _groupRepo;
    private readonly IUserRepo _userRepo;

    [ObservableProperty]
    private int _schoolId;

    [ObservableProperty]
    private string _groupName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<User> _teachers = new();

    [ObservableProperty]
    private User? _selectedTeacher;

    public GroupCreateViewModel(IGroupRepo groupRepo, IUserRepo userRepo)
    {
        _groupRepo = groupRepo;
        _userRepo = userRepo;
    }

    partial void OnSchoolIdChanged(int value)
    {
        LoadTeachersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadTeachers()
    {
        try
        {
            Teachers.Clear();

            IEnumerable<User> users = await _userRepo.GetAllAsync();

            foreach (User user in users)
            {
                if (user.SchoolId == SchoolId && user.UserType == UserType.Docent)
                {
                    Teachers.Add(user);
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Fout",
                ex.Message,
                "OK");
        }
    }

    [RelayCommand]
    private async Task CreateGroup()
    {
        if (string.IsNullOrWhiteSpace(GroupName))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Fout",
                "Groepsnaam is verplicht.",
                "OK");
            return;
        }

        try
        {
            Group group = new Group
            {
                Name = GroupName.Trim(),
                SchoolId = SchoolId,
                TeacherId = SelectedTeacher?.Id,
                TeacherName = SelectedTeacher?.Username ?? string.Empty
            };

            await _groupRepo.CreateAsync(group);

            await Application.Current.MainPage.DisplayAlert(
                "Gelukt",
                "Groep is aangemaakt.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Fout",
                ex.Message,
                "OK");
        }
    }
}
